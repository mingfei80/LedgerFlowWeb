namespace LedgerFlowWeb.Domain.Enums;

/// <summary>
/// Status of a skipped transaction record
/// </summary>
public enum SkippedTransactionStatus
{
    /// <summary>
    /// Transaction was skipped and remains unimported
    /// </summary>
    Skipped = 1,

    /// <summary>
    /// Transaction was manually force-imported
    /// </summary>
    ForceImported = 2,

    /// <summary>
    /// User reviewed and decided to ignore this transaction
    /// </summary>
    Ignored = 3
}
