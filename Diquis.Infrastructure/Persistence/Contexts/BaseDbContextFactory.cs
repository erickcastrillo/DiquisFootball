using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Diquis.Infrastructure.Persistence.Contexts
{
    /// <summary>
    /// Factory for creating instances of <see cref="BaseDbContext"/> at design time.
    /// Required for Entity Framework Core tools such as migrations, especially in multi-database scenarios.
    /// </summary>
    public class BaseDbContextFactory : IDesignTimeDbContextFactory<BaseDbContext>
    {
        /// <summary>
        /// Creates a new instance of <see cref="BaseDbContext"/> using configuration from <c>appsettings.json</c>.
        /// </summary>
        /// <param name="args">Arguments passed by the design-time tool (not used).</param>
        /// <returns>A configured <see cref="BaseDbContext"/> instance.</returns>
        public BaseDbContext CreateDbContext(string[] args)
        {
            // Build the configuration by reading from the appsettings.json file (requires Microsoft.Extensions.Configuration.Json Nuget Package)
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Retrieve the connection string from the configuration
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            DbContextOptionsBuilder<BaseDbContext> optionsBuilder = new();
            _ = optionsBuilder.UseNpgsql(connectionString);
            return new BaseDbContext(optionsBuilder.Options);
        }
    }
}
