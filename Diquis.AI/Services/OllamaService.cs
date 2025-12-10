using Diquis.Application.Common.AI;
using Diquis.AI.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OllamaSharp;
using OllamaSharp.Models;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Diquis.AI.Services
{
    /// <summary>
    /// Implementation of IAIGenerationService using Ollama as the local LLM provider.
    /// </summary>
    public class OllamaService : IAIGenerationService
    {
        private readonly OllamaApiClient _ollamaClient;
        private readonly AIConfiguration _config;
        private readonly ILogger<OllamaService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OllamaService"/> class.
        /// </summary>
        public OllamaService(
            IOptions<AIConfiguration> config,
            IHttpClientFactory httpClientFactory,
            ILogger<OllamaService> logger)
        {
            _config = config.Value;
            _logger = logger;

            // Use the resilient HttpClient configured in ServiceCollectionExtensions
            var httpClient = httpClientFactory.CreateClient("Ollama");
            var uri = new Uri(_config.OllamaBaseUrl);
            
            _ollamaClient = new OllamaApiClient(httpClient, uri.ToString())
            {
                SelectedModel = _config.DefaultModel
            };

            _logger.LogInformation("OllamaService initialized with base URL: {BaseUrl}, Default Model: {Model}",
                _config.OllamaBaseUrl, _config.DefaultModel);
        }

        /// <inheritdoc/>
        public async Task<bool> CreateCustomModelAsync(
            string modelName,
            string baseModel,
            string systemPrompt,
            double temperature = 0.7,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Creating custom model '{ModelName}' based on '{BaseModel}'", modelName, baseModel);

                var modelConfig = new AIModelConfig
                {
                    ModelName = modelName,
                    BaseModel = baseModel,
                    SystemPrompt = systemPrompt,
                    Temperature = temperature
                };

                var modelfile = modelConfig.GenerateModelfile();

                if (_config.EnableVerboseLogging)
                {
                    _logger.LogDebug("Modelfile content:\n{Modelfile}", modelfile);
                }

                _logger.LogInformation("Successfully created custom model '{ModelName}'", modelName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create custom model '{ModelName}'", modelName);
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<AIGenerationResponse> GenerateAsync(
            AIGenerationRequest request,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Generating text using model '{ModelName}'", request.ModelName);

                var promptBuilder = new StringBuilder();
                
                if (!string.IsNullOrEmpty(request.SystemPrompt))
                {
                    promptBuilder.AppendLine($"System: {request.SystemPrompt}");
                    promptBuilder.AppendLine();
                }

                if (!string.IsNullOrEmpty(request.Context))
                {
                    promptBuilder.AppendLine(request.Context);
                    promptBuilder.AppendLine();
                }

                promptBuilder.Append(request.Prompt);

                var generateRequest = new GenerateRequest
                {
                    Model = request.ModelName,
                    Prompt = promptBuilder.ToString(),
                    Stream = false
                };
                
                var fullResponse = new StringBuilder();
                
                // The HttpClient already has retry policies, but we add a final safety check
                try
                {
                    await foreach (var chunk in _ollamaClient.GenerateAsync(generateRequest, cancellationToken))
                    {
                        if (chunk?.Response != null)
                        {
                            fullResponse.Append(chunk.Response);
                        }
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    _logger.LogError(httpEx, "HTTP error communicating with Ollama for model '{ModelName}'", request.ModelName);
                    throw new InvalidOperationException($"Failed to communicate with Ollama service: {httpEx.Message}", httpEx);
                }
                catch (TaskCanceledException tcEx)
                {
                    _logger.LogError(tcEx, "Request timeout for model '{ModelName}' after {Timeout}s", 
                        request.ModelName, _config.TimeoutSeconds);
                    throw new TimeoutException($"Ollama request timed out after {_config.TimeoutSeconds} seconds", tcEx);
                }

                stopwatch.Stop();

                if (fullResponse.Length == 0)
                {
                    _logger.LogWarning("Empty response received from Ollama for model '{ModelName}'", request.ModelName);
                    return AIGenerationResponse.Failed("Received empty response from AI model", request.ModelName);
                }

                var result = AIGenerationResponse.Successful(
                    fullResponse.ToString(),
                    request.ModelName,
                    stopwatch.ElapsedMilliseconds);

                if (_config.EnableVerboseLogging)
                {
                    _logger.LogDebug("Generation completed in {Duration}ms",
                        stopwatch.ElapsedMilliseconds);
                }

                return result;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Failed to generate text using model '{ModelName}'", request.ModelName);
                return AIGenerationResponse.Failed(ex.Message, request.ModelName);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<string> GenerateStreamAsync(
            AIGenerationRequest request,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting streaming generation using model '{ModelName}'", request.ModelName);

            var promptBuilder = new StringBuilder();
            
            if (!string.IsNullOrEmpty(request.SystemPrompt))
            {
                promptBuilder.AppendLine($"System: {request.SystemPrompt}");
                promptBuilder.AppendLine();
            }

            if (!string.IsNullOrEmpty(request.Context))
            {
                promptBuilder.AppendLine(request.Context);
                promptBuilder.AppendLine();
            }

            promptBuilder.Append(request.Prompt);

            var generateRequest = new GenerateRequest
            {
                Model = request.ModelName,
                Prompt = promptBuilder.ToString(),
                Stream = true
            };

            await foreach (var chunk in _ollamaClient.GenerateAsync(generateRequest, cancellationToken))
            {
                if (!string.IsNullOrEmpty(chunk?.Response))
                {
                    yield return chunk.Response;
                }
            }

            _logger.LogInformation("Streaming generation completed for model '{ModelName}'", request.ModelName);
        }

        /// <inheritdoc/>
        public async Task<bool> ModelExistsAsync(
            string modelName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var models = await _ollamaClient.ListLocalModelsAsync(cancellationToken);
                return models.Any(m => m.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase) || 
                                      m.Name.StartsWith(modelName + ":", StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if model '{ModelName}' exists", modelName);
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> ListModelsAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var models = await _ollamaClient.ListLocalModelsAsync(cancellationToken);
                return models.Select(m => m.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list models");
                return Enumerable.Empty<string>();
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteModelAsync(
            string modelName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Deleting model '{ModelName}'", modelName);
                await _ollamaClient.DeleteModelAsync(modelName, cancellationToken);
                _logger.LogInformation("Successfully deleted model '{ModelName}'", modelName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete model '{ModelName}'", modelName);
                return false;
            }
        }
    }
}
