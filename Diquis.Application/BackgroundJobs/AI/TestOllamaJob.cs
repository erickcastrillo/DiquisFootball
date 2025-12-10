using Diquis.Application.Common.AI;
using Microsoft.Extensions.Logging;

namespace Diquis.Application.BackgroundJobs.AI
{
    /// <summary>
    /// Test job for verifying Ollama integration.
    /// Sends a simple prompt and logs the response.
    /// </summary>
    public class TestOllamaJob
    {
        private readonly IAIGenerationService _aiService;
        private readonly ILogger<TestOllamaJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestOllamaJob"/> class.
        /// </summary>
        public TestOllamaJob(
            IAIGenerationService aiService,
            ILogger<TestOllamaJob> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        /// <summary>
        /// Executes the test job with a simple prompt.
        /// </summary>
        /// <param name="prompt">The prompt to send to Ollama (optional).</param>
        /// <param name="modelName">The model to use (optional, defaults to configured default).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(string? prompt = null, string? modelName = null)
        {
            try
            {
                _logger.LogInformation("========== OLLAMA TEST JOB STARTED ==========");
                
                // Default prompt if none provided
                prompt ??= "Explain Clean Architecture in 3 concise paragraphs.";
                modelName ??= "llama2:latest"; // Default model

                _logger.LogInformation("Model: {ModelName}", modelName);
                _logger.LogInformation("Prompt: {Prompt}", prompt);
                _logger.LogInformation("Checking if model exists...");

                // Check if model exists
                var modelExists = await _aiService.ModelExistsAsync(modelName);
                
                if (!modelExists)
                {
                    _logger.LogWarning("Model '{ModelName}' not found locally", modelName);
                    _logger.LogInformation("Available models:");
                    
                    var availableModels = await _aiService.ListModelsAsync();
                    foreach (var model in availableModels)
                    {
                        _logger.LogInformation("  - {Model}", model);
                    }
                    
                    _logger.LogError("Please pull the '{ModelName}' model first using: ollama pull {ModelName}", modelName, modelName);
                    return;
                }

                _logger.LogInformation("Model '{ModelName}' found. Generating response...", modelName);
                _logger.LogInformation("---------------------------------------------------");

                // Create AI request
                var request = new AIGenerationRequest
                {
                    ModelName = modelName,
                    Prompt = prompt,
                    Temperature = 0.7,
                    MaxTokens = 500,
                    Metadata = new Dictionary<string, string>
                    {
                        { "JobType", "TestOllamaJob" },
                        { "ExecutedAt", DateTime.UtcNow.ToString("O") }
                    }
                };

                // Generate response
                var response = await _aiService.GenerateAsync(request);

                if (response.Success)
                {
                    _logger.LogInformation("✅ OLLAMA RESPONSE RECEIVED:");
                    _logger.LogInformation("---------------------------------------------------");
                    _logger.LogInformation("{Response}", response.GeneratedText);
                    _logger.LogInformation("---------------------------------------------------");
                    _logger.LogInformation("Duration: {Duration}ms", response.DurationMs);
                    _logger.LogInformation("Tokens: {Tokens}", response.TokensGenerated ?? 0);
                    _logger.LogInformation("Model: {Model}", response.ModelName);
                }
                else
                {
                    _logger.LogError("❌ OLLAMA REQUEST FAILED:");
                    _logger.LogError("Error: {Error}", response.ErrorMessage);
                }

                _logger.LogInformation("========== OLLAMA TEST JOB COMPLETED ==========");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ UNEXPECTED ERROR IN OLLAMA TEST JOB");
                throw;
            }
        }
    }
}
