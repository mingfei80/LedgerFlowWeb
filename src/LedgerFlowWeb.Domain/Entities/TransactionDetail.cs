using LedgerFlowWeb.Domain.Enums;

namespace LedgerFlowWeb.Domain.Entities;

/// <summary>
/// Platform-specific transaction fields that do not fit the normalized transaction model.
/// </summary>
public class TransactionDetail
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Linked normalized transaction.
    /// </summary>
    public long TransactionId { get; set; }

    /// <summary>
    /// Source platform for this detail row.
    /// </summary>
    public Platform Platform { get; set; }

    /// <summary>
    /// Source row category, such as buy/sell/dividend/fee.
    /// </summary>
    public string? SourceRowType { get; set; }

    /// <summary>
    /// Original source row payload serialized to JSON.
    /// </summary>
    public string? RawRowJson { get; set; }

    /// <summary>
    /// Parsed source-specific fields serialized to JSON.
    /// </summary>
    public string? ParsedExtrasJson { get; set; }

    /// <summary>
    /// Timestamp when this detail row was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Transaction Transaction { get; set; } = null!;
}
