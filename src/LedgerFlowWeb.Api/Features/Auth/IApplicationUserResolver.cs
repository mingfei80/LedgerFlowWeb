using LedgerFlowWeb.Domain.Entities;

namespace LedgerFlowWeb.Api.Features.Auth;

public interface IApplicationUserResolver
{
    Task<User> EnsureCurrentUserAsync(CancellationToken cancellationToken = default);
    Guid GetRequiredCurrentUserId();
}
