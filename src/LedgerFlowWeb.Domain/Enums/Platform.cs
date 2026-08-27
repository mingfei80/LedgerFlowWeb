namespace LedgerFlowWeb.Domain.Enums;

/// <summary>
/// Trading platform providers
/// </summary>
public enum Platform
{
    /// <summary>
    /// Hargreaves Lansdown (handles both ISA and LISA accounts)
    /// </summary>
    HL = 1,

    /// <summary>
    /// Interactive Group (IG)
    /// </summary>
    IG = 2,

    /// <summary>
    /// Trading 212
    /// </summary>
    T212 = 3
}
