using LedgerFlowWeb.Api.Features.Auth;
using LedgerFlowWeb.Infrastructure.Persistance;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlowWeb.Api.Features.Import.IG;

public static class ImportIGTransactions
{
    public static IEndpointRouteBuilder MapIGImportEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/import/ig", ImportAsync)
            .WithName("ImportIGTransactions")
            .WithDescription("Imports IG transactions from CSV file upload")
            .WithTags("Import")
            .RequireAuthorization()
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data");

        return app;
    }

    private static async Task<IResult> ImportAsync(
        [FromForm] IFormFile file,
        [FromForm] int accountId,
        AppDbContext dbContext,
        IApplicationUserResolver userResolver,
        IIGImportService importService,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return Results.BadRequest(new { message = "CSV file is required." });
        }

        var appUser = await userResolver.EnsureCurrentUserAsync(cancellationToken);

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await importService.ImportAsync(appUser.Id, accountId, stream, file.FileName, cancellationToken);
            return Results.Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new { message = "Account not found for the current user." });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }
}
