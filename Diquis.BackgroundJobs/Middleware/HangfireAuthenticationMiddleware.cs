namespace Diquis.BackgroundJobs.Middleware;

/// <summary>
/// Middleware to handle authentication redirects for Hangfire dashboard
/// </summary>
public class HangfireAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public HangfireAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if this is a request to the root path or Hangfire dashboard
        if (context.Request.Path == "/" || context.Request.Path.StartsWithSegments("/hangfire"))
        {
            // Check if user is not authenticated
            if (context.User.Identity?.IsAuthenticated != true)
            {
                // Redirect to login with the current path as return URL
                string returnUrl = context.Request.Path + context.Request.QueryString;
                context.Response.Redirect($"/Identity/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
                return;
            }

            // Check if user is authenticated but doesn't have the required role
            if (!context.User.IsInRole("root"))
            {
                context.Response.Redirect("/Identity/Account/AccessDenied");
                return;
            }

            // User is authenticated and authorized, allow the request to proceed
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method to add the Hangfire authentication middleware
/// </summary>
public static class HangfireAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseHangfireAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HangfireAuthenticationMiddleware>();
    }
}
