using Hangfire.Dashboard;

namespace Diquis.BackgroundJobs.Middleware;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string _role;

    public HangfireAuthorizationFilter(string role)
    {
        _role = role;
    }

    public bool Authorize(DashboardContext context)
    {
        HttpContext httpContext = context.GetHttpContext();
        
        // Only check authentication and authorization, don't redirect
        // Redirection is handled by the middleware
        return httpContext.User.Identity?.IsAuthenticated == true 
               && httpContext.User.IsInRole(_role);
    }
}
