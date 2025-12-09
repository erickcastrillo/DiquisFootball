using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Console;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Diquis.BackgroundJobs.Data;
using Diquis.BackgroundJobs.Areas.Identity.Data;
using Diquis.BackgroundJobs.Services;
using Diquis.Infrastructure.BackgroundJobs.Telemetry;
using Diquis.Infrastructure.BackgroundJobs;
using Diquis.Infrastructure.Persistence.Contexts;
using Diquis.Application.Common.Notifications;
using Diquis.Application.Common;
using Diquis.Domain.Entities.Common;
using Diquis.Infrastructure.Services;

namespace Diquis.BackgroundJobs.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureJobs(this IServiceCollection services, IConfiguration configuration)
    {
        // Get connection strings
        string hangfireConnectionString = configuration.GetConnectionString("HangfireConnection") ?? throw new InvalidOperationException("Connection string 'HangfireConnection' not found.");
        string defaultConnectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        string redisConnectionString = configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";

        // Add DbContext for Identity (BackgroundJobs authentication)
        _ = services.AddDbContext<DiquisInfrastructureJobsContext>(options => options.UseNpgsql(hangfireConnectionString));

        // Add Identity for BackgroundJobs UI authentication
        _ = services.AddIdentity<DiquisBackgroundJobsUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<DiquisInfrastructureJobsContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

        // Add tenant/user service for background jobs (no user context)
        _ = services.AddScoped<ICurrentTenantUserService, BackgroundJobTenantUserService>();

        // Add BaseDbContext for job access to tenants (uses main application database)
        _ = services.AddDbContext<BaseDbContext>(options => options.UseNpgsql(defaultConnectionString));

        // Add ApplicationDbContext for tenant-specific operations (uses main application database)
        _ = services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(defaultConnectionString));

        // Add UserManager for ApplicationUser (used by ProvisionTenantJob)
        _ = services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<BaseDbContext>();

        // Add Hangfire services with deferred storage configuration (uses Hangfire database)
        _ = services.AddHangfire((serviceProvider, config) => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(hangfireConnectionString);
            })
            .UseConsole()
            .UseFilter(new HangfireActivityFilter()));

        // Add the processing server as IHostedService
        _ = services.AddHangfireServer(options =>
        {
            options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
            options.ServerCheckInterval = TimeSpan.FromSeconds(30);
            options.WorkerCount = 10;
        });

        // Add SignalR with Redis backplane for cross-process messaging
        _ = services.AddSignalR()
            .AddStackExchangeRedis(redisConnectionString, options =>
            {
                options.Configuration.ChannelPrefix = "Diquis";
            });

        // Register SignalR-based notification service with Redis backplane support
        _ = services.AddScoped<INotificationService, SignalRNotificationService>();

        // Register background jobs
        _ = services.AddScoped<TestJob>();
        _ = services.AddScoped<ProvisionTenantJob>();
        _ = services.AddScoped<UpdateTenantJob>();

        // Add framework services.
        _ = services.AddControllersWithViews();
        _ = services.AddRazorPages();

        // Add Authentication and Authorization
        _ = services.AddAuthentication();
        _ = services.AddAuthorization();
    }
}
