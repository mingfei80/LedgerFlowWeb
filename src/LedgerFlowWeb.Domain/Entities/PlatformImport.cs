using LedgerFlowWeb.Domain.Enums;

namespace LedgerFlowWeb.Domain.Entities;

/// <summary>
/// Raw archive of imported platform rows for traceability and re-checks.
/// </summary>
public class PlatformImport
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Related import operation.
    /// </summary>
    public long ImportLogId { get; set; }

    /// <summary>
    /// Owner of the imported row.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Account targeted by this import row.
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Source platform.
    /// </summary>
    public Platform Platform { get; set; }

    /// <summary>
    /// Row number from the source file, when available.
    /// </summary>
    public int? SourceRowNumber { get; set; }

    /// <summary>
    /// Source row category, such as buy/sell/dividend/fee.
    /// </summary>
    public string? SourceRowType { get; set; }

    /// <summary>
    /// Original source row payload serialized to JSON.
    /// </summary>
    public string RawRowJson { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when this archive record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ImportLog ImportLog { get; set; } = null!;
    public User User { get; set; } = null!;
    public Account Account { get; set; } = null!;
}
