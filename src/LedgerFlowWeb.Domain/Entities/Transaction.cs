using LedgerFlowWeb.Domain.Enums;

namespace LedgerFlowWeb.Domain.Entities;

/// <summary>
/// Financial transaction (buy, sell, dividend, etc.)
/// </summary>
public class Transaction
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Owner of this transaction
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Account this transaction belongs to
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Security being traded (nullable for cash transactions)
    /// </summary>
    public int? SecurityId { get; set; }

    /// <summary>
    /// Type of transaction
    /// </summary>
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// Transaction date
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Settlement date (nullable)
    /// </summary>
    public DateTime? SettlementDate { get; set; }

    /// <summary>
    /// Number of shares/units (can be negative for sells)
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Price per unit
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total transaction value (negative for purchases, positive for sales in HL format)
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>
    /// Transaction currency
    /// </summary>
    public string Currency { get; set; } = "GBP";

    /// <summary>
    /// Exchange rate used (if multi-currency)
    /// </summary>
    public decimal? ExchangeRate { get; set; }

    /// <summary>
    /// Fees associated with this transaction
    /// </summary>
    public decimal? Fees { get; set; }

    /// <summary>
    /// Platform's original reference ID/code
    /// </summary>
    public string? PlatformReference { get; set; }

    /// <summary>
    /// Description from CSV
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Additional metadata (JSON for platform-specific data)
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// When this transaction was created in the system
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Who created this transaction
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public Security? Security { get; set; }
    public ICollection<TransactionDetail> Details { get; set; } = new List<TransactionDetail>();
}
