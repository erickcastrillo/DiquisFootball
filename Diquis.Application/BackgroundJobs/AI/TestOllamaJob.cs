using Diquis.Application.Common;
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
        private readonly IPromptService _promptService;
        private readonly ILogger<TestOllamaJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestOllamaJob"/> class.
        /// </summary>
        public TestOllamaJob(
            IAIGenerationService aiService,
            IPromptService promptService,
            ILogger<TestOllamaJob> logger)
        {
            _aiService = aiService;
            _promptService = promptService;
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
                
                // Use configured prompt if none provided
                string systemPrompt;
                string userPrompt;

                if (string.IsNullOrEmpty(prompt))
                {
                    // Use the CleanArchitecture default test prompt
                    (systemPrompt, userPrompt) = _promptService.RenderPrompt("CleanArchitecture");
                    _logger.LogInformation("Using configured 'CleanArchitecture' prompt template");
                }
                else
                {
                    // Use the generic TestOllama prompt with custom user input
                    var variables = new Dictionary<string, string>
                    {
                        { "prompt", prompt }
                    };
                    (systemPrompt, userPrompt) = _promptService.RenderPrompt("TestOllama", variables);
                    _logger.LogInformation("Using configured 'TestOllama' prompt template with custom prompt");
                }

                modelName ??= "llama2"; // Default model

                _logger.LogInformation("Model: {ModelName}", modelName);
                _logger.LogInformation("System Prompt: {SystemPrompt}", systemPrompt);
                _logger.LogInformation("User Prompt: {UserPrompt}", userPrompt);
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

                // Get temperature and max tokens from prompt configuration
                var promptTemplate = _promptService.GetPrompt(string.IsNullOrEmpty(prompt) ? "CleanArchitecture" : "TestOllama");
                
                // Create AI request
                var request = new AIGenerationRequest
                {
                    ModelName = modelName,
                    Prompt = userPrompt,
                    SystemPrompt = systemPrompt,
                    Temperature = promptTemplate?.Temperature ?? 0.7,
                    MaxTokens = promptTemplate?.MaxTokens ?? 500,
                    Metadata = new Dictionary<string, string>
                    {
                        { "JobType", "TestOllamaJob" },
                        { "ExecutedAt", DateTime.UtcNow.ToString("O") },
                        { "PromptTemplate", string.IsNullOrEmpty(prompt) ? "CleanArchitecture" : "TestOllama" }
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
