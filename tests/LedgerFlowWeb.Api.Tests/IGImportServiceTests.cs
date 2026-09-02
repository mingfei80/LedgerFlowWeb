using System.IO;
using System.Text;
using LedgerFlowWeb.Api.Features.Import.IG;
using LedgerFlowWeb.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LedgerFlowWeb.Api.Tests;

public class IGImportServiceTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task MapRow_ParseSimpleCsv_ImportsSuccessfully()
    {
        using var db = CreateInMemoryDbContext();

        // Arrange: create user and account
        var user = new LedgerFlowWeb.Domain.Entities.User { Id = Guid.NewGuid(), Email = "t@test", Name = "Test", CreatedAt = DateTime.UtcNow };
        db.Users.Add(user);
        var account = new LedgerFlowWeb.Domain.Entities.Account { Id = 1, UserId = user.Id, Name = "IG Account", Platform = LedgerFlowWeb.Domain.Enums.Platform.IG, AccountType = LedgerFlowWeb.Domain.Enums.AccountType.Real, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        db.Accounts.Add(account);
        await db.SaveChangesAsync();

        var csv = new StringBuilder();
        csv.AppendLine("TextDate,Summary,MarketName,Period,ProfitAndLoss,Transaction type,Reference,Open level,Close level,Size,Currency,PL Amount,Cash transaction,DateUtc,OpenDateUtc,CurrencyIsoCode");
        csv.AppendLine("01/01/23,Test Bought,FOO 100 @ 10, , ,BUY,REF123, , ,100,GBP,100.00, ,2023-01-01T00:00:00Z, ,GBP");

        var service = new IGImportService(db);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

        // Act
        var result = await service.ImportAsync(user.Id, account.Id, stream, "test.csv");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Created);
        Assert.Equal(0, result.Skipped);
    }
}
