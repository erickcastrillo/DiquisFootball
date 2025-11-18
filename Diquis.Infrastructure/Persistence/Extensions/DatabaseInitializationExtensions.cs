using Diquis.Domain.Entities.Multitenancy;
using Diquis.Infrastructure.Persistence.Contexts;
using Diquis.Infrastructure.Persistence.Initializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diquis.Infrastructure.Persistence.Extensions
{
    /// <summary>
    /// Provides extension methods for initializing and migrating tenant databases.
    /// </summary>
    public static class DatabaseInitializationExtensions
    {
        /// <summary>
        /// Adds and migrates tenant databases for a multi-tenant application.
        /// Applies migrations to the default database (using <typeparamref name="TB"/> and <typeparamref name="TA"/>)
        /// and to each tenant's database (using <typeparamref name="TA"/>).
        /// </summary>
        /// <typeparam name="T">The <see cref="TenantDbContext"/> type used to query tenant information.</typeparam>
        /// <typeparam name="TB">The <see cref="BaseDbContext"/> type for the main database (identity, tenants, configuration).</typeparam>
        /// <typeparam name="TA">The <see cref="ApplicationDbContext"/> type for application data (products, etc.).</typeparam>
        /// <param name="services">The service collection to add the contexts to.</param>
        /// <param name="configuration">The application configuration, used to retrieve connection strings.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        /// <remarks>
        /// This method:
        /// <list type="number">
        /// <item>Applies pending migrations to the default database using <typeparamref name="TB"/>.</item>
        /// <item>Seeds the default database with tenant, admin user, and roles if empty.</item>
        /// <item>Retrieves all tenants from the default database using <typeparamref name="T"/>.</item>
        /// <item>For each tenant, applies pending migrations to the tenant's database using <typeparamref name="TA"/>.</item>
        /// </list>
        /// </remarks>
        public static IServiceCollection AddAndMigrateTenantDatabases<T, TB, TA>(this IServiceCollection services, IConfiguration configuration)
            where T : TenantDbContext
            where TB : BaseDbContext
            where TA : ApplicationDbContext
        {
            // apply migrations to default DB from BaseDBContext (identity & tenant tables) 
            using IServiceScope scopeBase = services.BuildServiceProvider().CreateScope();
            TB baseDbContext = scopeBase.ServiceProvider.GetRequiredService<TB>();
            if (baseDbContext.Database.GetPendingMigrations().Any())
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Applying BaseDb Migrations.");
                Console.ResetColor();
                baseDbContext.Database.Migrate(); // apply any pending migrations
            }

            // apply migrations from ApplicationDBContext, to all DBs - the default DB and any tenant DBs
            string defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
            if (baseDbContext.Database.CanConnect())
            {
                DbInitializer.SeedTenantAdminAndRoles(baseDbContext); // first check default DB -- seed tenant, admin user, and roles if empty

                using IServiceScope scopeTenant = services.BuildServiceProvider().CreateScope();
                T tenantDbContext = scopeTenant.ServiceProvider.GetRequiredService<T>();
                List<Tenant> tenantsInDb = tenantDbContext.Tenants.ToList(); // get a list of tenants - using the auxiliary context TenantDbContext 

                foreach (Tenant tenant in tenantsInDb) // loop through list of tenants, using the unique connection string when there is one
                {
                    string connectionString = string.IsNullOrEmpty(tenant.ConnectionString) ? defaultConnectionString : tenant.ConnectionString;

                    // apply migrations to all DBs from ApplicationDBContext (all application tables) 
                    using IServiceScope scopeApplication = services.BuildServiceProvider().CreateScope();
                    TA dbContext = scopeApplication.ServiceProvider.GetRequiredService<TA>();
                    dbContext.Database.SetConnectionString(connectionString);
                    if (dbContext.Database.GetPendingMigrations().Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Applying ApplicationDb Migrations for '{tenant.Id}' tenant.");
                        Console.ResetColor();
                        dbContext.Database.Migrate();
                    }
                }
            }
            return services;
        }
    }
}

