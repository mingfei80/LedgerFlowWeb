namespace LedgerFlowWeb.Domain.Enums;

/// <summary>
/// Types of financial transactions
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Purchase of securities
    /// </summary>
    Buy = 1,

    /// <summary>
    /// Sale of securities
    /// </summary>
    Sell = 2,

    /// <summary>
    /// Dividend payment received
    /// </summary>
    Dividend = 3,

    /// <summary>
    /// Interest payment received
    /// </summary>
    Interest = 4,

    /// <summary>
    /// Platform or management fee
    /// </summary>
    Fee = 5,

    /// <summary>
    /// Transfer between accounts or cash transfer
    /// </summary>
    Transfer = 6,

    /// <summary>
    /// Withholding tax deducted (e.g., on dividends)
    /// </summary>
    WithholdingTax = 7,

    /// <summary>
    /// Currency conversion fee
    /// </summary>
    CurrencyConversionFee = 8
}
