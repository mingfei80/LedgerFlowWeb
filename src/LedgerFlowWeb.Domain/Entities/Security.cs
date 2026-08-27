namespace LedgerFlowWeb.Domain.Entities;

/// <summary>
/// Security (stock, share, ETF, etc.)
/// </summary>
public class Security
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ISIN code (International Securities Identification Number)
    /// </summary>
    public string? ISIN { get; set; }

    /// <summary>
    /// Ticker symbol
    /// </summary>
    public string? Ticker { get; set; }

    /// <summary>
    /// Full name of the security
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Currency the security is traded in
    /// </summary>
    public string Currency { get; set; } = "GBP";

    /// <summary>
    /// Exchange/market (e.g., LSE, NASDAQ)
    /// </summary>
    public string? Exchange { get; set; }

    /// <summary>
    /// Additional metadata (JSON)
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// When this security was first added to the system
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
