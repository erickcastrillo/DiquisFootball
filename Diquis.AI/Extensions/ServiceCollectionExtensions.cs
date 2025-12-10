using Diquis.Application.Common.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using System.Reflection;

namespace Diquis.AI.Extensions
{
    /// <summary>
    /// Extension methods for registering AI services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds AI services to the service collection.
        /// Loads AI configuration from appsettings.AI.json in the Diquis.AI assembly directory.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddAIServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Get the directory where Diquis.AI assembly is located
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            var aiConfigPath = Path.Combine(assemblyDirectory!, "appsettings.AI.json");

            // Build a new configuration that includes the AI settings
            var configBuilder = new ConfigurationBuilder();
            
            // Add the existing configuration
            if (configuration is IConfigurationRoot configRoot)
            {
                foreach (var source in configRoot.Providers)
                {
                    // We can't directly copy sources, so we'll just use the passed configuration
                }
            }

            // Add AI configuration file from Diquis.AI assembly directory
            if (File.Exists(aiConfigPath))
            {
                configBuilder.AddJsonFile(aiConfigPath, optional: false, reloadOnChange: true);
            }
            else
            {
                // Fallback: try to load from current directory (for development scenarios)
                configBuilder.AddJsonFile("appsettings.AI.json", optional: false, reloadOnChange: true);
            }

            var aiConfig = configBuilder.Build();

            // Bind configuration - merge with existing configuration
            services.Configure<AIConfiguration>(options =>
            {
                // First try to bind from the passed configuration (for override scenarios)
                configuration.GetSection(AIConfiguration.SectionName).Bind(options);
                
                // Then bind from AI-specific config (will override if keys exist)
                aiConfig.GetSection(AIConfiguration.SectionName).Bind(options);
            });

            // Register Ollama service as the primary implementation
            services.AddScoped<IAIGenerationService, Services.OllamaService>();

            // Register prompt service (defined in Application layer to avoid circular dependency)
            services.AddScoped<IPromptService, PromptService>();

            // Register background jobs
            services.AddScoped<Application.BackgroundJobs.AI.ProcessSingleDataForAIJob>();
            services.AddScoped<Application.BackgroundJobs.AI.ProcessBatchDataForAIJob>();
            services.AddScoped<Application.BackgroundJobs.AI.TestOllamaJob>();

            // Optional: Add HttpClient for Ollama with retry policies
            services.AddHttpClient("Ollama", client =>
            {
                var aiConfiguration = aiConfig
                    .GetSection(AIConfiguration.SectionName)
                    .Get<AIConfiguration>();

                client.BaseAddress = new Uri(aiConfiguration?.OllamaBaseUrl ?? "http://localhost:11434");
                client.Timeout = TimeSpan.FromSeconds(aiConfiguration?.TimeoutSeconds ?? 300);
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
