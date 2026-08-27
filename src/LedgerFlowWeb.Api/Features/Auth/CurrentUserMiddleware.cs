namespace LedgerFlowWeb.Api.Features.Auth;

public sealed class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApplicationUserResolver userResolver)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await userResolver.EnsureCurrentUserAsync(context.RequestAborted);
        }

        await _next(context);
    }
}
