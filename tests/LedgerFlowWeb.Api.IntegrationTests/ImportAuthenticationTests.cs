using System.Net;
using System.Net.Http.Headers;
using LedgerFlowWeb.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LedgerFlowWeb.Api.IntegrationTests;

public sealed class ImportAuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ImportAuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ImportIg_WithProvidedBearerToken_ShouldNotReturnUnauthorized()
    {
        var token = Environment.GetEnvironmentVariable("LEDGERFLOW_TEST_BEARER_TOKEN");
        Assert.False(string.IsNullOrWhiteSpace(token), "Set LEDGERFLOW_TEST_BEARER_TOKEN to a real Azure AD access token before running this test.");

        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var form = new MultipartFormDataContent();
        using var csvContent = new StringContent("Date,Market Name,Direction,Open Size,Open Level,Closing Level,Profit/Loss\n");
        form.Add(csvContent, "file", "ig.csv");
        form.Add(new StringContent("1"), "accountId");

        var response = await client.PostAsync("/api/import/ig", form);

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
