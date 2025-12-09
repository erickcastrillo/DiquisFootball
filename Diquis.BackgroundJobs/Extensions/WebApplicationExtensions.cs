using Diquis.BackgroundJobs.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Diquis.BackgroundJobs.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using (IServiceScope scope = app.Services.CreateScope())
        {
            IServiceProvider services = scope.ServiceProvider;
            ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
            IConfiguration configuration = services.GetRequiredService<IConfiguration>();

            try
            {
                // Verify PostgreSQL connection first
                string connectionString = configuration.GetConnectionString("HangfireConnection") 
                    ?? throw new InvalidOperationException("Connection string 'HangfireConnection' not found.");
                
                logger.LogInformation("Verifying PostgreSQL connection...");
                await VerifyPostgreSqlConnectionAsync(connectionString, logger);
                logger.LogInformation("PostgreSQL connection verified successfully.");

                // Apply EF Core migrations
                DiquisInfrastructureJobsContext context = services.GetRequiredService<DiquisInfrastructureJobsContext>();
                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");

                // Seed the database with initial users and roles
                logger.LogInformation("Seeding database with initial data...");
                await JobsDbInitializer.SeedRolesAndUsers(services);
                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }

    private static async Task VerifyPostgreSqlConnectionAsync(string connectionString, ILogger logger)
    {
        int maxRetries = 5;
        int delayMilliseconds = 2000;

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                logger.LogInformation("Successfully connected to PostgreSQL database.");
                return;
            }
            catch (NpgsqlException ex)
            {
                if (i == maxRetries - 1)
                {
                    logger.LogError(ex, "Failed to connect to PostgreSQL after {MaxRetries} attempts.", maxRetries);
                    throw;
                }

                logger.LogWarning("PostgreSQL connection attempt {Attempt} failed. Retrying in {Delay}ms...", i + 1, delayMilliseconds);
                await Task.Delay(delayMilliseconds);
            }
        }
    }
}
