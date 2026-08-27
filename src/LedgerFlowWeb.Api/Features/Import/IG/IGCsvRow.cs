namespace LedgerFlowWeb.Api.Features.Import.IG;

public sealed class IGCsvRow
{
    public string? TextDate { get; set; }
    public string? Summary { get; set; }
    public string? MarketName { get; set; }
    public string? Period { get; set; }
    public string? ProfitAndLoss { get; set; }
    public string? TransactionTypeCode { get; set; }
    public string? Reference { get; set; }
    public string? OpenLevel { get; set; }
    public string? CloseLevel { get; set; }
    public string? Size { get; set; }
    public string? Currency { get; set; }
    public string? PLAmount { get; set; }
    public string? CashTransaction { get; set; }
    public string? DateUtc { get; set; }
    public string? OpenDateUtc { get; set; }
    public string? CurrencyIsoCode { get; set; }
}
