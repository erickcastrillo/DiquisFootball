using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Diquis.WebApi.Middleware;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string _role;

    public HangfireAuthorizationFilter(string role)
    {
        _role = role;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.IsInRole(_role);
    }
}
