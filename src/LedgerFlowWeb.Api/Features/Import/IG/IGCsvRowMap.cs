using CsvHelper.Configuration;

namespace LedgerFlowWeb.Api.Features.Import.IG;

public sealed class IGCsvRowMap : ClassMap<IGCsvRow>
{
    public IGCsvRowMap()
    {
        Map(m => m.TextDate).Name("TextDate");
        Map(m => m.Summary).Name("Summary");
        Map(m => m.MarketName).Name("MarketName");
        Map(m => m.Period).Name("Period");
        Map(m => m.ProfitAndLoss).Name("ProfitAndLoss");
        Map(m => m.TransactionTypeCode).Name("Transaction type");
        Map(m => m.Reference).Name("Reference");
        Map(m => m.OpenLevel).Name("Open level");
        Map(m => m.CloseLevel).Name("Close level");
        Map(m => m.Size).Name("Size");
        Map(m => m.Currency).Name("Currency");
        Map(m => m.PLAmount).Name("PL Amount");
        Map(m => m.CashTransaction).Name("Cash transaction");
        Map(m => m.DateUtc).Name("DateUtc");
        Map(m => m.OpenDateUtc).Name("OpenDateUtc");
        Map(m => m.CurrencyIsoCode).Name("CurrencyIsoCode");
    }
}
