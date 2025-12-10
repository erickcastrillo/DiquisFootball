using Diquis.AI.Configuration;
using Diquis.AI.Services;
using Diquis.Application.Common.AI;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Diquis.AI.Extensions
{
    /// <summary>
    /// Extension methods for registering AI services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds AI services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddAIServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Bind configuration
            services.Configure<AIConfiguration>(
                configuration.GetSection(AIConfiguration.SectionName));

            // Register Ollama service as the primary implementation
            services.AddScoped<IAIGenerationService, OllamaService>();

            // Register background jobs
            services.AddScoped<Application.BackgroundJobs.AI.ProcessSingleDataForAIJob>();
            services.AddScoped<Application.BackgroundJobs.AI.ProcessBatchDataForAIJob>();
            services.AddScoped<Application.BackgroundJobs.AI.TestOllamaJob>();

            // Optional: Add HttpClient for Ollama with retry policies
            services.AddHttpClient("Ollama", client =>
            {
                var aiConfig = configuration
                    .GetSection(AIConfiguration.SectionName)
                    .Get<AIConfiguration>();

                client.BaseAddress = new Uri(aiConfig?.OllamaBaseUrl ?? "http://localhost:11434");
                client.Timeout = TimeSpan.FromSeconds(aiConfig?.TimeoutSeconds ?? 300);
            })
            .AddStandardResilienceHandler(options =>
            {
                // Configure retry policy
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromSeconds(1);
                options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
                
                // Configure circuit breaker
                // SamplingDuration must be at least double the AttemptTimeout (2 * 600s = 1200s minimum)
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(1200);
                options.CircuitBreaker.FailureRatio = 0.5;
                options.CircuitBreaker.MinimumThroughput = 5;
                options.AttemptTimeout = new HttpTimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromMinutes(10) // Increase timeout for LLM responses
                };
                options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromMinutes(10)
                };
            });

            return services;
        }
    }
}
