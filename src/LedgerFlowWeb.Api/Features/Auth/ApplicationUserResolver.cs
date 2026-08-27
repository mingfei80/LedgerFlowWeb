using System.Security.Claims;
using LedgerFlowWeb.Domain.Entities;
using LedgerFlowWeb.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlowWeb.Api.Features.Auth;

public sealed class ApplicationUserResolver : IApplicationUserResolver
{
    private const string MicrosoftProvider = "Microsoft";
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationUserResolver(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetRequiredCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User
            ?? throw new InvalidOperationException("No active HTTP context user was found.");

        var userIdClaim = user.FindFirstValue("app_user_id");
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        throw new InvalidOperationException("Current user has not been resolved into an application user yet.");
    }

    public async Task<User> EnsureCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var principal = _httpContextAccessor.HttpContext?.User
            ?? throw new InvalidOperationException("No active HTTP context user was found.");

        if (principal.Identity?.IsAuthenticated != true)
        {
            throw new InvalidOperationException("Authenticated Microsoft identity is required.");
        }

        var externalSubjectId = principal.FindFirstValue("oid")
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub")
            ?? throw new InvalidOperationException("No stable external subject identifier claim was found.");

        var email = principal.FindFirstValue("preferred_username")
            ?? principal.FindFirstValue(ClaimTypes.Email)
            ?? string.Empty;

        var displayName = principal.FindFirstValue("name")
            ?? principal.Identity?.Name
            ?? email;

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(
                u => u.ExternalProvider == MicrosoftProvider && u.ExternalSubjectId == externalSubjectId,
                cancellationToken);

        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = string.IsNullOrWhiteSpace(email) ? $"unknown-{Guid.NewGuid():N}@local" : email,
                Name = string.IsNullOrWhiteSpace(displayName) ? "Unknown User" : displayName,
                ExternalProvider = MicrosoftProvider,
                ExternalSubjectId = externalSubjectId,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
        }
        else
        {
            user.Email = string.IsNullOrWhiteSpace(email) ? user.Email : email;
            user.Name = string.IsNullOrWhiteSpace(displayName) ? user.Name : displayName;
            user.LastLoginAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        AddResolvedUserIdClaim(principal, user.Id);
        return user;
    }

    private static void AddResolvedUserIdClaim(ClaimsPrincipal principal, Guid userId)
    {
        if (principal.Identity is not ClaimsIdentity identity)
        {
            return;
        }

        var existingClaim = identity.FindFirst("app_user_id");
        if (existingClaim is not null)
        {
            if (existingClaim.Value == userId.ToString())
            {
                return;
            }

            identity.RemoveClaim(existingClaim);
        }

        identity.AddClaim(new Claim("app_user_id", userId.ToString()));
    }
}
