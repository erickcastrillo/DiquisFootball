using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace Diquis.Infrastructure.BackgroundJobs;

/// <summary>
/// Test job to verify Hangfire is working correctly with OpenTelemetry tracing
/// </summary>
public class TestJob
{
    private readonly ILogger<TestJob> _logger;

    public TestJob(ILogger<TestJob> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Simple test job that logs messages and simulates some work
    /// </summary>
    public async Task ExecuteAsync(string message, PerformContext? context = null)
    {
        _logger.LogInformation("Test job started with message: {Message}", message);
        _logger.LogInformation("Starting test job at {Time}", DateTime.UtcNow);
        
        // Simulate some work
        for (int i = 1; i <= 5; i++)
        {
            await Task.Delay(1000);
            _logger.LogInformation("Test job progress: {Progress}%", i * 20);
        }
        
        _logger.LogInformation("Test job completed successfully at {Time}", DateTime.UtcNow);
    }

    /// <summary>
    /// Test job that demonstrates error handling
    /// </summary>
    public Task ExecuteWithErrorAsync(PerformContext? context = null)
    {
        _logger.LogWarning("Test job that will fail - intentional error for testing");
        
        throw new InvalidOperationException("This is a test exception to verify error tracking in OpenTelemetry");
    }
}
