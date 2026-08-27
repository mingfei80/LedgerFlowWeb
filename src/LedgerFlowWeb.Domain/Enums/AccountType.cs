namespace LedgerFlowWeb.Domain.Enums;

/// <summary>
/// Account type (tax wrapper)
/// </summary>
public enum AccountType
{
    /// <summary>
    /// Individual Savings Account (UK tax-free wrapper)
    /// </summary>
    ISA = 1,

    /// <summary>
    /// Lifetime ISA (UK government bonus scheme)
    /// </summary>
    LISA = 2,

    /// <summary>
    /// Standard taxable trading account
    /// </summary>
    Standard = 3,

    /// <summary>
    /// Other account types
    /// </summary>
    Other = 4
}
