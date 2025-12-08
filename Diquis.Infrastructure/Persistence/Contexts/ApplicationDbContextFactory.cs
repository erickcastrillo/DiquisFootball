using Diquis.Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Diquis.Infrastructure.Persistence.Contexts
{
    /// <summary>
    /// Factory for creating instances of <see cref="ApplicationDbContext"/> at design time.
    /// Required for Entity Framework Core tools such as migrations.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ApplicationDbContext"/> using configuration from <c>appsettings.json</c>.
        /// </summary>
        /// <param name="args">Arguments passed by the design-time tool (not used).</param>
        /// <returns>A configured <see cref="ApplicationDbContext"/> instance.</returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Navigate to the WebApi project directory to find appsettings.json
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Diquis.WebApi");
            
            // Build the configuration by reading from the appsettings.json file
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Retrieve the connection string from the configuration
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
            _ = optionsBuilder.UseNpgsql(connectionString);

            // Create a mock/design-time implementation of ICurrentTenantUserService
            var designTimeTenantService = new DesignTimeCurrentTenantUserService(connectionString);

            return new ApplicationDbContext(designTimeTenantService, optionsBuilder.Options);
        }

        /// <summary>
        /// Design-time implementation of <see cref="ICurrentTenantUserService"/> for EF Core migrations.
        /// Provides default values when no HTTP context is available.
        /// </summary>
        private class DesignTimeCurrentTenantUserService : ICurrentTenantUserService
        {
            public DesignTimeCurrentTenantUserService(string connectionString)
            {
                ConnectionString = connectionString;
                TenantId = "root"; // Default tenant for migrations
                UserId = null;
            }

            public string? ConnectionString { get; set; }
            public string? TenantId { get; set; }
            public string? UserId { get; set; }

            public Task<bool> SetTenantUser(string tenant)
            {
                // Not used at design time
                return Task.FromResult(true);
            }
        }
    }
}
