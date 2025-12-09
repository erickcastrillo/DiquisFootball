using Diquis.Application.Common.Notifications;
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Domain.Enums;
using Diquis.Infrastructure.Multitenancy.DTOs;
using Diquis.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Diquis.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Background job for provisioning a new tenant, including database creation and admin user setup.
    /// </summary>
    public class ProvisionTenantJob
    {
        private readonly BaseDbContext _baseDbContext;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ProvisionTenantJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProvisionTenantJob"/> class.
        /// </summary>
        public ProvisionTenantJob(
            BaseDbContext baseDbContext,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment,
            INotificationService notificationService,
            ILogger<ProvisionTenantJob> logger)
        {
            _baseDbContext = baseDbContext;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _environment = environment;
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Executes the tenant provisioning process.
        /// </summary>
        /// <param name="tenantId">The ID of the tenant to provision.</param>
        /// <param name="request">The tenant creation request details.</param>
        /// <param name="initiatingUserId">The ID of the user who initiated the provisioning.</param>
        public async Task ExecuteAsync(string tenantId, CreateTenantRequest request, string initiatingUserId)
        {
            var tenant = await _baseDbContext.Tenants.FindAsync(tenantId);
            if (tenant == null)
            {
                _logger.LogError("Tenant {TenantId} not found for provisioning", tenantId);
                return;
            }

            try
            {
                tenant.Status = ProvisioningStatus.Provisioning;
                tenant.LastProvisioningAttempt = DateTime.UtcNow;
                await _baseDbContext.SaveChangesAsync();

                _logger.LogInformation("Starting provisioning for tenant {TenantId}", tenantId);

                // Create admin user
                var user = new ApplicationUser
                {
                    UserName = request.AdminEmail + "." + tenant.Id,
                    FirstName = "Default",
                    LastName = "Admin",
                    Email = request.AdminEmail,
                    EmailConfirmed = true,
                    TenantId = tenant.Id
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create admin user: {errors}");
                }

                // Provision isolated database if needed
                if (request.HasIsolatedDatabase && !string.IsNullOrEmpty(tenant.ConnectionString))
                {
                    _logger.LogInformation("Provisioning isolated database for tenant {TenantId}", tenantId);

                    using var scopeTenant = _serviceProvider.CreateScope();
                    var dbContext = scopeTenant.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.SetConnectionString(tenant.ConnectionString);

                    try
                    {
                        if (_environment.IsDevelopment())
                        {
                            // Create database if it doesn't exist (without schema)
                            var canConnect = await dbContext.Database.CanConnectAsync();
                            
                            if (!canConnect)
                            {
                                _logger.LogInformation("Creating database for tenant {TenantId}", tenantId);
                                
                                // Extract database name from connection string
                                var builder = new Npgsql.NpgsqlConnectionStringBuilder(tenant.ConnectionString);
                                var databaseName = builder.Database;
                                
                                // Connect to default postgres database to create the tenant database
                                builder.Database = "postgres";
                                var masterConnectionString = builder.ToString();
                                
                                using var masterScope = _serviceProvider.CreateScope();
                                var masterContext = masterScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                                masterContext.Database.SetConnectionString(masterConnectionString);
                                
                                // Create the database
                                await masterContext.Database.ExecuteSqlRawAsync($"CREATE DATABASE \"{databaseName}\"");
                                _logger.LogInformation("Database {DatabaseName} created for tenant {TenantId}", databaseName, tenantId);
                            }

                            // Now apply migrations to the newly created database
                            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                            if (pendingMigrations.Any())
                            {
                                _logger.LogInformation("Applying {Count} migrations for tenant {TenantId}", pendingMigrations.Count(), tenantId);
                                await dbContext.Database.MigrateAsync();
                            }
                            else
                            {
                                _logger.LogInformation("No pending migrations for tenant {TenantId}", tenantId);
                            }
                        }
                        else
                        {
                            // Production environment - implement cloud-specific logic here
                            // Azure Elastic Pool, AWS RDS, etc.
                            _logger.LogInformation("Production database provisioning for tenant {TenantId}", tenantId);
                            
                            // For cloud providers, databases are usually pre-provisioned
                            // Just run migrations
                            await dbContext.Database.MigrateAsync();
                        }
                    }
                    catch (Exception dbEx)
                    {
                        _logger.LogError(dbEx, "Failed to provision database for tenant {TenantId}", tenantId);
                        throw new Exception($"Database provisioning failed: {dbEx.Message}", dbEx);
                    }
                }

                await _userManager.AddToRoleAsync(user, "academy_owner");

                tenant.Status = ProvisioningStatus.Active;
                tenant.ProvisioningError = null;
                await _baseDbContext.SaveChangesAsync();

                _logger.LogInformation("Tenant {TenantId} provisioned successfully", tenantId);

                // Notify user of success
                await _notificationService.NotifyTenantCreatedAsync(initiatingUserId, tenant.Id, tenant.Name);
                
                _logger.LogInformation("Tenant creation notification sent for {TenantId}", tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Provisioning failed for tenant {TenantId}", tenantId);

                tenant.Status = ProvisioningStatus.Failed;
                tenant.ProvisioningError = ex.Message;
                await _baseDbContext.SaveChangesAsync();

                await _notificationService.NotifyTenantCreationFailedAsync(initiatingUserId, ex.Message);

                throw; // Re-throw to let Hangfire handle retries
            }
        }
    }
}
