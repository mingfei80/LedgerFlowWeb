using LedgerFlowWeb.Domain.Enums;

namespace LedgerFlowWeb.Domain.Entities;

/// <summary>
/// Detailed record of each transaction that was skipped during import
/// </summary>
public class SkippedTransaction
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Which import attempt this belongs to
    /// </summary>
    public long ImportLogId { get; set; }

    /// <summary>
    /// Owner of this transaction
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Target account
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Platform
    /// </summary>
    public Platform Platform { get; set; }

    /// <summary>
    /// Why was this transaction skipped?
    /// </summary>
    public SkipReason SkipReason { get; set; }

    /// <summary>
    /// The existing transaction it matched (if duplicate)
    /// </summary>
    public long? MatchedTransactionId { get; set; }

    /// <summary>
    /// When was it skipped
    /// </summary>
    public DateTime SkippedDate { get; set; }

    /// <summary>
    /// Status of this skipped transaction
    /// </summary>
    public SkippedTransactionStatus Status { get; set; }

    // Original transaction data
    public DateTime TransactionDate { get; set; }
    public TransactionType TransactionType { get; set; }
    public string SecurityName { get; set; } = string.Empty;
    public string? SecurityISIN { get; set; }
    public string? SecurityTicker { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
    public string Currency { get; set; } = "GBP";

    /// <summary>
    /// Full CSV row preserved as JSON
    /// </summary>
    public string? OriginalCsvRow { get; set; }

    /// <summary>
    /// Optional manual notes
    /// </summary>
    public string? Notes { get; set; }

    // Navigation properties
    public ImportLog ImportLog { get; set; } = null!;
    public User User { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public Transaction? MatchedTransaction { get; set; }
}
