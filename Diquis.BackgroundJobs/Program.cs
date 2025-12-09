using Diquis.BackgroundJobs.Extensions;
using Diquis.BackgroundJobs.Middleware;
using Diquis.Infrastructure.Hubs;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Antiforgery;
using OpenTelemetry.Trace;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// All service configurations are now in this extension method
builder.Services.ConfigureJobs(builder.Configuration);

// Setup OpenTelemetry Tracing
builder.Services.AddOpenTelemetry().WithTracing(tracingBuilder =>
{
    _ = tracingBuilder
        // Add Hangfire instrumentation via ActivitySource
        .AddSource("Diquis.Hangfire")
        // Configure ASP.NET Core Instrumentation
        .AddAspNetCoreInstrumentation()
        // Configure OpenTelemetry Protocol (OTLP) Exporter
        .AddOtlpExporter();
});

WebApplication app = builder.Build();

// Initialize database (migrations + seeding)
await app.InitializeDatabaseAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Add Hangfire authentication middleware AFTER authentication/authorization
// This handles authentication checks and redirects for "/" and "/hangfire" paths
app.UseHangfireAuthentication();

// Map SignalR Hub - CRITICAL: Must use the same hub as WebApi
app.MapHub<NotificationHub>("/hubs/notifications");

// Map root path to a simple HTML page with link to Hangfire
app.MapGet("/", (HttpContext context, IAntiforgery antiforgery) =>
{
    var userName = context.User.Identity?.Name ?? "Unknown User";
    var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value ?? userName;
    
    // Generate anti-forgery token for logout form
    var tokens = antiforgery.GetAndStoreTokens(context);
    var requestToken = tokens.RequestToken!;
    
    return Results.Content($@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Diquis Jobs Server</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 1rem;
        }}
        .container {{
            text-align: center;
            background: white;
            padding: 3rem;
            border-radius: 1rem;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            max-width: 500px;
            width: 100%;
        }}
        .user-info {{
            position: absolute;
            top: 1rem;
            right: 1rem;
            background: white;
            padding: 0.75rem 1.5rem;
            border-radius: 0.5rem;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            display: flex;
            align-items: center;
            gap: 1rem;
        }}
        .user-email {{
            color: #333;
            font-size: 0.9rem;
            font-weight: 500;
        }}
        .logout-btn {{
            padding: 0.5rem 1rem;
            background: #dc3545;
            color: white;
            text-decoration: none;
            border-radius: 0.375rem;
            font-weight: 600;
            font-size: 0.875rem;
            transition: all 0.3s ease;
            border: none;
            cursor: pointer;
        }}
        .logout-btn:hover {{
            background: #c82333;
            transform: translateY(-1px);
            box-shadow: 0 2px 8px rgba(220, 53, 69, 0.3);
        }}
        h1 {{
            color: #333;
            margin-bottom: 1rem;
            font-size: 2rem;
        }}
        .subtitle {{
            color: #666;
            margin-bottom: 2rem;
            font-size: 1.1rem;
        }}
        .dashboard-btn {{
            display: inline-block;
            padding: 1rem 2rem;
            background: #667eea;
            color: white;
            text-decoration: none;
            border-radius: 0.5rem;
            font-weight: 600;
            font-size: 1rem;
            transition: all 0.3s ease;
            margin-bottom: 1.5rem;
        }}
        .dashboard-btn:hover {{
            background: #764ba2;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.2);
        }}
        .info {{
            margin-top: 2rem;
            padding-top: 2rem;
            border-top: 1px solid #e0e0e0;
        }}
        .info-item {{
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 0.75rem;
            margin: 0.5rem 0;
            background: #f8f9fa;
            border-radius: 0.375rem;
        }}
        .info-label {{
            color: #666;
            font-weight: 600;
            font-size: 0.9rem;
        }}
        .info-value {{
            color: #333;
            font-size: 0.9rem;
        }}
        .status-badge {{
            display: inline-block;
            padding: 0.25rem 0.75rem;
            background: #28a745;
            color: white;
            border-radius: 1rem;
            font-size: 0.8rem;
            font-weight: 600;
        }}
        @media (max-width: 768px) {{
            .user-info {{
                position: static;
                margin-bottom: 2rem;
                flex-direction: column;
            }}
        }}
    </style>
</head>
<body>
    <div class='user-info'>
        <span class='user-email'>👤 {userEmail}</span>
        <form method='post' action='/Identity/Account/Logout' style='margin: 0;'>
            <input type='hidden' name='__RequestVerificationToken' value='{requestToken}' />
            <input type='hidden' name='returnUrl' value='/Identity/Account/Login' />
            <button type='submit' class='logout-btn'>Logout</button>
        </form>
    </div>
    <div class='container'>
        <h1>🚀 Diquis Jobs Server</h1>
        <p class='subtitle'>Background job processing and monitoring dashboard</p>
        <a href='/hangfire' class='dashboard-btn'>Open Hangfire Dashboard →</a>
        
        <div class='info'>
            <div class='info-item'>
                <span class='info-label'>Status</span>
                <span class='status-badge'>● Online</span>
            </div>
            <div class='info-item'>
                <span class='info-label'>Logged in as</span>
                <span class='info-value'>{userEmail}</span>
            </div>
            <div class='info-item'>
                <span class='info-label'>Role</span>
                <span class='info-value'>Root Administrator</span>
            </div>
        </div>
    </div>
</body>
</html>
", "text/html");
});

// Map Razor Pages for Identity UI
app.MapRazorPages();
// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Map Hangfire dashboard with custom authorization
app.MapHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter("root") }
});

app.Run();
