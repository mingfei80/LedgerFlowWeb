namespace LedgerFlowWeb.Domain.Enums;

/// <summary>
/// Status of a CSV import operation
/// </summary>
public enum ImportStatus
{
    /// <summary>
    /// All transactions imported successfully
    /// </summary>
    Success = 1,

    /// <summary>
    /// Some transactions imported, some skipped or failed
    /// </summary>
    PartialSuccess = 2,

    /// <summary>
    /// Import failed completely
    /// </summary>
    Failed = 3
}
