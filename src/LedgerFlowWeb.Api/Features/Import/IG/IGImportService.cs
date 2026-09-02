using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using CsvHelper;
using LedgerFlowWeb.Domain.Entities;
using LedgerFlowWeb.Domain.Enums;
using LedgerFlowWeb.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlowWeb.Api.Features.Import.IG;

public sealed class IGImportService : IIGImportService
{
    private readonly AppDbContext _dbContext;

    public IGImportService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ImportResult> ImportAsync(Guid userId, int accountId, Stream csvStream, string fileName, CancellationToken cancellationToken = default)
    {
        var startedAt = DateTime.UtcNow;

        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId, cancellationToken);

        if (account is null)
        {
            throw new KeyNotFoundException("Account not found for the current user.");
        }

        if (account.Platform != Platform.IG)
        {
            throw new InvalidOperationException("Selected account is not an IG account.");
        }

        var importLog = new ImportLog
        {
            UserId = userId,
            AccountId = account.Id,
            Platform = Platform.IG,
            FileName = fileName,
            ImportDate = startedAt,
            Status = ImportStatus.Success,
            ImportedCount = 0,
            SkippedCount = 0,
            ErrorCount = 0
        };

        _dbContext.ImportLogs.Add(importLog);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var errors = new List<string>();
        var rows = await ReadRowsAsync(csvStream, cancellationToken);

        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowNumber = i + 2;
            var rawRowJson = JsonSerializer.Serialize(row);

            _dbContext.PlatformImports.Add(new PlatformImport
            {
                ImportLogId = importLog.Id,
                UserId = userId,
                AccountId = account.Id,
                Platform = Platform.IG,
                SourceRowNumber = rowNumber,
                SourceRowType = row.TransactionTypeCode,
                RawRowJson = rawRowJson,
                CreatedAt = DateTime.UtcNow
            });

            var mapped = MapRow(row, userId, account.Id);
            if (!mapped.Success || mapped.Transaction is null)
            {
                importLog.ErrorCount++;
                importLog.SkippedCount++;
                errors.Add($"Row {rowNumber}: {mapped.ErrorMessage}");

                _dbContext.SkippedTransactions.Add(new SkippedTransaction
                {
                    ImportLogId = importLog.Id,
                    UserId = userId,
                    AccountId = account.Id,
                    Platform = Platform.IG,
                    SkipReason = SkipReason.InvalidData,
                    SkippedDate = DateTime.UtcNow,
                    Status = SkippedTransactionStatus.Skipped,
                    TransactionDate = mapped.FallbackDate ?? DateTime.UtcNow,
                    TransactionType = mapped.FallbackType ?? TransactionType.Transfer,
                    SecurityName = mapped.FallbackSecurityName,
                    SecurityISIN = null,
                    SecurityTicker = null,
                    Quantity = 0,
                    UnitPrice = 0,
                    TotalValue = 0,
                    Currency = mapped.FallbackCurrency ?? "GBP",
                    OriginalCsvRow = rawRowJson,
                    Notes = mapped.ErrorMessage
                });

                continue;
            }

            mapped.Transaction.SecurityId = await ResolveSecurityIdAsync(_dbContext, mapped, cancellationToken);

            var duplicate = await _dbContext.Transactions
                .FirstOrDefaultAsync(
                    t => t.UserId == mapped.Transaction.UserId
                         && t.AccountId == mapped.Transaction.AccountId
                         && t.TransactionDate == mapped.Transaction.TransactionDate
                         && t.TransactionType == mapped.Transaction.TransactionType
                         && t.SecurityId == mapped.Transaction.SecurityId
                         && t.Quantity == mapped.Transaction.Quantity
                         && t.UnitPrice == mapped.Transaction.UnitPrice,
                    cancellationToken);

            if (duplicate is not null)
            {
                importLog.SkippedCount++;

                _dbContext.SkippedTransactions.Add(new SkippedTransaction
                {
                    ImportLogId = importLog.Id,
                    UserId = userId,
                    AccountId = account.Id,
                    Platform = Platform.IG,
                    SkipReason = SkipReason.DuplicateTransaction,
                    MatchedTransactionId = duplicate.Id,
                    SkippedDate = DateTime.UtcNow,
                    Status = SkippedTransactionStatus.Skipped,
                    TransactionDate = mapped.Transaction.TransactionDate,
                    TransactionType = mapped.Transaction.TransactionType,
                    SecurityName = mapped.SecurityName,
                    SecurityISIN = mapped.SecurityIsin,
                    SecurityTicker = mapped.SecurityTicker,
                    Quantity = mapped.Transaction.Quantity,
                    UnitPrice = mapped.Transaction.UnitPrice,
                    TotalValue = mapped.Transaction.TotalValue,
                    Currency = mapped.Transaction.Currency,
                    OriginalCsvRow = rawRowJson,
                    Notes = "Duplicate transaction by idempotency key."
                });

                continue;
            }

            _dbContext.Transactions.Add(mapped.Transaction);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _dbContext.TransactionDetails.Add(new TransactionDetail
            {
                TransactionId = mapped.Transaction.Id,
                Platform = Platform.IG,
                SourceRowType = row.TransactionTypeCode,
                RawRowJson = rawRowJson,
                ParsedExtrasJson = JsonSerializer.Serialize(new
                {
                    row.TextDate,
                    row.Summary,
                    row.MarketName,
                    row.Period,
                    row.ProfitAndLoss,
                    row.TransactionTypeCode,
                    row.Reference,
                    row.OpenLevel,
                    row.CloseLevel,
                    row.Size,
                    row.Currency,
                    row.PLAmount,
                    row.CashTransaction,
                    row.DateUtc,
                    row.OpenDateUtc,
                    row.CurrencyIsoCode
                }),
                CreatedAt = DateTime.UtcNow
            });

            importLog.ImportedCount++;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        importLog.DurationMs = (int)(DateTime.UtcNow - startedAt).TotalMilliseconds;
        importLog.ErrorCount = importLog.ErrorCount;
        importLog.Status = ImportStatus.Success;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ImportResult
        {
            ImportLogId = importLog.Id,
            Created = importLog.ImportedCount,
            Skipped = importLog.SkippedCount,
            Errors = errors,
            DurationMs = importLog.DurationMs
        };
    }

    private static async Task<List<IGCsvRow>> ReadRowsAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<IGCsvRowMap>();

        var rows = new List<IGCsvRow>();
        await foreach (var row in csv.GetRecordsAsync<IGCsvRow>(cancellationToken))
        {
            rows.Add(row);
        }

        return rows;
    }

    private static async Task<int?> ResolveSecurityIdAsync(
        AppDbContext dbContext,
        MappedRow mapped,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mapped.SecurityName)
            && string.IsNullOrWhiteSpace(mapped.SecurityTicker)
            && string.IsNullOrWhiteSpace(mapped.SecurityIsin))
        {
            return null;
        }

        Security? security = null;

        if (!string.IsNullOrWhiteSpace(mapped.SecurityIsin))
        {
            security = await dbContext.Securities
                .FirstOrDefaultAsync(s => s.ISIN == mapped.SecurityIsin, cancellationToken);
        }

        if (security is null && !string.IsNullOrWhiteSpace(mapped.SecurityTicker))
        {
            security = await dbContext.Securities
                .FirstOrDefaultAsync(s => s.Ticker == mapped.SecurityTicker, cancellationToken);
        }

        if (security is null && !string.IsNullOrWhiteSpace(mapped.SecurityName))
        {
            security = await dbContext.Securities
                .FirstOrDefaultAsync(s => s.Name == mapped.SecurityName, cancellationToken);
        }

        if (security is null)
        {
            security = new Security
            {
                Name = mapped.SecurityName,
                Ticker = mapped.SecurityTicker,
                ISIN = mapped.SecurityIsin,
                Currency = mapped.Transaction?.Currency ?? "GBP",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.Securities.Add(security);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return security.Id;
    }

    private static MappedRow MapRow(IGCsvRow row, Guid userId, int accountId)
    {
        if (!TryParseDate(row.DateUtc, out var transactionDate) && !TryParseDate(row.TextDate, out transactionDate))
        {
            return MappedRow.Fail("Date could not be parsed.", fallbackCurrency: row.CurrencyIsoCode);
        }

        var transactionType = MapTransactionType(row);
        var (qty, unitPrice) = ParseQuantityAndPrice(row.MarketName);
        var totalValue = ParseMoney(row.PLAmount ?? row.ProfitAndLoss) ?? 0m;
        var exchangeRate = ExtractExchangeRate(row.MarketName);

        var securityName = ExtractSecurityName(row.MarketName);
        var securityTicker = ExtractTicker(row.MarketName);

        var transaction = new Transaction
        {
            UserId = userId,
            AccountId = accountId,
            TransactionType = transactionType,
            TransactionDate = transactionDate,
            Quantity = qty,
            UnitPrice = unitPrice,
            TotalValue = totalValue,
            Currency = string.IsNullOrWhiteSpace(row.CurrencyIsoCode) ? "GBP" : row.CurrencyIsoCode.Trim(),
            ExchangeRate = exchangeRate,
            PlatformReference = row.Reference,
            Description = row.MarketName,
            Metadata = JsonSerializer.Serialize(new
            {
                row.Summary,
                row.TransactionTypeCode,
                row.ProfitAndLoss,
                row.OpenLevel,
                row.CloseLevel,
                row.Size,
                row.CashTransaction
            }),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        return MappedRow.Ok(transaction, securityName, securityTicker, null);
    }

    private static TransactionType MapTransactionType(IGCsvRow row)
    {
        var summary = row.Summary?.Trim() ?? string.Empty;
        var marketName = row.MarketName?.Trim() ?? string.Empty;
        var code = row.TransactionTypeCode?.Trim().ToUpperInvariant() ?? string.Empty;

        if (summary.Contains("Dividend", StringComparison.OrdinalIgnoreCase)
            || marketName.Contains("DIVIDEND", StringComparison.OrdinalIgnoreCase))
        {
            return TransactionType.Dividend;
        }

        if (summary.Contains("Interest", StringComparison.OrdinalIgnoreCase)
            || marketName.Contains("Interest", StringComparison.OrdinalIgnoreCase))
        {
            return TransactionType.Interest;
        }

        if (summary.Contains("SDRT", StringComparison.OrdinalIgnoreCase)
            || marketName.Contains("Section 31 Fee", StringComparison.OrdinalIgnoreCase)
            || marketName.Contains("Fee", StringComparison.OrdinalIgnoreCase))
        {
            return TransactionType.Fee;
        }

        if (code == "BUY" || summary.Contains("Bought", StringComparison.OrdinalIgnoreCase) || marketName.Contains("Bought"))
        {
            return TransactionType.Buy;
        }

        if (code == "SELL" || summary.Contains("Sold", StringComparison.OrdinalIgnoreCase) || marketName.Contains("Sold"))
        {
            return TransactionType.Sell;
        }

        return TransactionType.Transfer;
    }

    private static (decimal Quantity, decimal UnitPrice) ParseQuantityAndPrice(string? marketName)
    {
        if (string.IsNullOrWhiteSpace(marketName))
        {
            return (0m, 0m);
        }

        var match = Regex.Match(marketName, @"@\s*([0-9,]+\.?[0-9]*)\s*@\s*([0-9,]+\.?[0-9]*)");
        if (!match.Success)
        {
            return (0m, 0m);
        }

        var quantity = ParseDecimalInvariant(match.Groups[1].Value) ?? 0m;
        var unitPrice = ParseDecimalInvariant(match.Groups[2].Value) ?? 0m;

        return (quantity, unitPrice);
    }

    private static decimal? ExtractExchangeRate(string? marketName)
    {
        if (string.IsNullOrWhiteSpace(marketName))
        {
            return null;
        }

        var match = Regex.Match(
            marketName,
            @"Converted\s+at\s+([0-9]+(?:\.[0-9]+)?)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        if (!match.Success)
        {
            return null;
        }

        return ParseDecimalInvariant(match.Groups[1].Value);
    }

    private static decimal? ParseMoney(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "-")
        {
            return null;
        }

        var normalized = value.Replace("£", string.Empty, StringComparison.Ordinal)
            .Replace("$", string.Empty, StringComparison.Ordinal)
            .Replace(",", string.Empty, StringComparison.Ordinal)
            .Trim();

        return ParseDecimalInvariant(normalized);
    }

    private static decimal? ParseDecimalInvariant(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return decimal.TryParse(
            value,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out var parsed)
            ? parsed
            : null;
    }

    private static bool TryParseDate(string? value, out DateTime date)
    {
        if (DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out date))
        {
            return true;
        }

        return DateTime.TryParseExact(
            value,
            "dd/MM/yy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out date);
    }

    private static string ExtractSecurityName(string? marketName)
    {
        if (string.IsNullOrWhiteSpace(marketName))
        {
            return "Unknown Security";
        }

        var cleaned = Regex.Replace(
            marketName,
            @"\s+(CONS|DIVIDEND)\s+[0-9]+(?:\.[0-9]+)?@[0-9]+(?:\.[0-9]+)?(?:\s+V[0-9A-Z:~\-]+)?(?:\s+Converted\s+at\s+[0-9]+(?:\.[0-9]+)?)?",
            string.Empty,
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            return marketName.Trim();
        }

        return cleaned.Trim();
    }

    private static string? ExtractTicker(string? marketName)
    {
        if (string.IsNullOrWhiteSpace(marketName))
        {
            return null;
        }

        var match = Regex.Match(
            marketName,
            @"\b([A-Z]{1,6})\s+(?:CONS|DIVIDEND)\b",
            RegexOptions.CultureInvariant);

        return match.Success ? match.Groups[1].Value : null;
    }

    private sealed record MappedRow(
        bool Success,
        string? ErrorMessage,
        Transaction? Transaction,
        string SecurityName,
        string? SecurityTicker,
        string? SecurityIsin,
        DateTime? FallbackDate,
        TransactionType? FallbackType,
        string? FallbackCurrency,
        string FallbackSecurityName)
    {
        public static MappedRow Ok(Transaction transaction, string securityName, string? ticker, string? isin) =>
            new(
                true,
                null,
                transaction,
                securityName,
                ticker,
                isin,
                null,
                null,
                transaction.Currency,
                securityName);

        public static MappedRow Fail(string message, string? fallbackCurrency) =>
            new(
                false,
                message,
                null,
                "Unknown Security",
                null,
                null,
                null,
                null,
                fallbackCurrency,
                "Unknown Security");
    }

    // ImportResult moved to ImportModels.cs
}
