using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Console;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Diquis.BackgroundJobs.Data;
using Diquis.BackgroundJobs.Areas.Identity.Data;
using Diquis.Infrastructure.BackgroundJobs.Telemetry;
using Diquis.Infrastructure.BackgroundJobs;

namespace Diquis.BackgroundJobs.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureJobs(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext for Identity
        string connectionString = configuration.GetConnectionString("HangfireConnection") ?? throw new InvalidOperationException("Connection string 'HangfireConnection' not found.");
        _ = services.AddDbContext<DiquisInfrastructureJobsContext>(options => options.UseNpgsql(connectionString));

        // Add Identity
        _ = services.AddIdentity<DiquisBackgroundJobsUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<DiquisInfrastructureJobsContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

        // Add Hangfire services with deferred storage configuration
        _ = services.AddHangfire((serviceProvider, config) => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(connectionString);
            })
            .UseConsole()
            .UseFilter(new HangfireActivityFilter()));

        // Add the processing server as IHostedService
        _ = services.AddHangfireServer(options =>
        {
            // Only start the server after the app has fully started
            options.ServerCheckInterval = TimeSpan.FromSeconds(30);
        });

        // Register test job
        _ = services.AddScoped<TestJob>();

        // Add framework services.
        _ = services.AddControllersWithViews();
        _ = services.AddRazorPages();

        // Add Authentication and Authorization
        _ = services.AddAuthentication();
        _ = services.AddAuthorization();
    }
}
