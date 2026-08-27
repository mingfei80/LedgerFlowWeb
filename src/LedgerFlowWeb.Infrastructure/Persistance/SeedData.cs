using LedgerFlowWeb.Domain.Entities;
namespace LedgerFlowWeb.Infrastructure.Persistance;

/// <summary>
/// Seed data for development and testing
/// </summary>
public static class SeedData
{
    public static void Seed(AppDbContext context)
    {
        // Seed only reference data in Phase 1.
        if (context.Currencies.Any())
            return;

        var currencies = new[]
        {
            new Currency { Code = "GBP", Name = "British Pound", Symbol = "£", DecimalPlaces = 2 },
            new Currency { Code = "USD", Name = "US Dollar", Symbol = "$", DecimalPlaces = 2 },
            new Currency { Code = "EUR", Name = "Euro", Symbol = "€", DecimalPlaces = 2 }
        };

        context.Currencies.AddRange(currencies);
        context.SaveChanges();
    }
}
