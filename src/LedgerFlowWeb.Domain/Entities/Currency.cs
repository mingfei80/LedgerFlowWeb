namespace LedgerFlowWeb.Domain.Entities;

/// <summary>
/// Currency reference data
/// </summary>
public class Currency
{
    /// <summary>
    /// ISO 4217 currency code (GBP, USD, EUR)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Currency name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Currency symbol (£, $, €)
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Number of decimal places
    /// </summary>
    public int DecimalPlaces { get; set; } = 2;
}
