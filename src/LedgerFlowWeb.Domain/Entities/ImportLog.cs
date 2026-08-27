using LedgerFlowWeb.Domain.Enums;

namespace LedgerFlowWeb.Domain.Entities;

/// <summary>
/// Audit log for each CSV import attempt
/// </summary>
public class ImportLog
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Who performed the import
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Which account was imported to
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Platform
    /// </summary>
    public Platform Platform { get; set; }

    /// <summary>
    /// Original CSV filename
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// When the import was performed
    /// </summary>
    public DateTime ImportDate { get; set; }

    /// <summary>
    /// Import status
    /// </summary>
    public ImportStatus Status { get; set; }

    /// <summary>
    /// Number of transactions successfully imported
    /// </summary>
    public int ImportedCount { get; set; }

    /// <summary>
    /// Number of transactions skipped (duplicates)
    /// </summary>
    public int SkippedCount { get; set; }

    /// <summary>
    /// Number of rows that had errors
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Error messages (JSON)
    /// </summary>
    public string? ErrorDetails { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public ICollection<SkippedTransaction> SkippedTransactions { get; set; } = new List<SkippedTransaction>();
    public ICollection<PlatformImport> PlatformImports { get; set; } = new List<PlatformImport>();
}
