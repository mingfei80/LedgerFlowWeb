using System.Security.Claims;
using LedgerFlowWeb.Api.Features.Auth;
using LedgerFlowWeb.Api.Features.Import.IG;
using LedgerFlowWeb.Infrastructure.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Scalar.AspNetCore;

namespace LedgerFlowWeb.Api;

public class Program
{
    const string AllowReactAppPolicy = "AllowReactApp";
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure database
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        var azureAdSection = builder.Configuration.GetSection("AzureAd");
        var clientId = azureAdSection["ClientId"];
        var configuredAudience = azureAdSection["Audience"];

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(azureAdSection);

        builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var validAudiences = new List<string>();

            if (!string.IsNullOrWhiteSpace(configuredAudience))
            {
                validAudiences.Add(configuredAudience);
            }

            if (!string.IsNullOrWhiteSpace(clientId))
            {
                validAudiences.Add(clientId);
                validAudiences.Add($"api://{clientId}");
            }

            options.TokenValidationParameters.ValidAudiences = validAudiences
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        });

        builder.Services.AddAuthorization();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IApplicationUserResolver, ApplicationUserResolver>();
        builder.Services.AddScoped<IIGImportService, IGImportService>();
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(AllowReactAppPolicy, policy =>
            {
                policy.WithOrigins("http://localhost:64003") // Your React app URL
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials(); // Essential if you pass tokens or credentials
            });
        });

        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, ct) =>
            {
                document.Components ??= new();
                document.Components.SecuritySchemes ??= new Dictionary<string, Microsoft.OpenApi.IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes["Bearer"] = new Microsoft.OpenApi.OpenApiSecurityScheme
                {
                    Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your Azure AD JWT Bearer token"
                };

                document.Security ??= new List<Microsoft.OpenApi.OpenApiSecurityRequirement>();
                document.Security.Add(new Microsoft.OpenApi.OpenApiSecurityRequirement
                {
                    [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document, "Bearer")] = new List<string>()
                });

                return Task.CompletedTask;
            });
        });

        var app = builder.Build();

        // Configure middleware
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api/import/ig"))
            {
                var authHeader = context.Request.Headers.Authorization.ToString();
                Console.WriteLine($"[PipelineDebug] {context.Request.Method} {context.Request.Path}, AuthHeaderPresent={!string.IsNullOrWhiteSpace(authHeader)}, Prefix={(authHeader.Length > 20 ? authHeader[..20] : authHeader)}");
            }

            await next();
        });

        app.UseCors(AllowReactAppPolicy);

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<CurrentUserMiddleware>();

        app.MapGet("/api/users/me", async (ClaimsPrincipal user, IApplicationUserResolver userResolver, CancellationToken cancellationToken) =>
        {
            var currentUser = await userResolver.EnsureCurrentUserAsync(cancellationToken);

            return Results.Ok(new
            {
                currentUser.Id,
                currentUser.Email,
                currentUser.Name,
                currentUser.ExternalProvider,
                currentUser.ExternalSubjectId,
                Claims = user.Claims.Select(claim => new { claim.Type, claim.Value })
            });
        })
        .RequireAuthorization();

        app.MapIGImportEndpoints();

        app.Run();
    }
}
