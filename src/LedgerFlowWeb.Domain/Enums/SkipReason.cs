namespace LedgerFlowWeb.Domain.Enums;

/// <summary>
/// Reason why a transaction was skipped during import
/// </summary>
public enum SkipReason
{
    /// <summary>
    /// Transaction already exists in database (duplicate)
    /// </summary>
    DuplicateTransaction = 1,

    /// <summary>
    /// Invalid or malformed data in CSV row
    /// </summary>
    InvalidData = 2,

    /// <summary>
    /// Security (stock/share) not found or could not be created
    /// </summary>
    MissingSecurity = 3,

    /// <summary>
    /// Account not found
    /// </summary>
    AccountNotFound = 4,

    /// <summary>
    /// Other unspecified reason
    /// </summary>
    Other = 5
}
