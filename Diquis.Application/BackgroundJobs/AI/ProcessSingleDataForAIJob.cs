using Diquis.Application.Common;
using Diquis.Application.Common.AI;
using Microsoft.Extensions.Logging;

namespace Diquis.Application.BackgroundJobs.AI
{
    /// <summary>
    /// Background job for processing individual AI generation tasks.
    /// This is the child job that processes one data item.
    /// </summary>
    public class ProcessSingleDataForAIJob
    {
        private readonly IAIGenerationService _aiService;
        private readonly IPromptService _promptService;
        private readonly IRepositoryAsync _repository;
        private readonly ILogger<ProcessSingleDataForAIJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSingleDataForAIJob"/> class.
        /// </summary>
        public ProcessSingleDataForAIJob(
            IAIGenerationService aiService,
            IPromptService promptService,
            IRepositoryAsync repository,
            ILogger<ProcessSingleDataForAIJob> logger)
        {
            _aiService = aiService;
            _promptService = promptService;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Executes the AI processing for a single data item using a configured prompt template.
        /// </summary>
        /// <param name="dataId">The ID of the data item to process.</param>
        /// <param name="modelName">The AI model to use.</param>
        /// <param name="promptKey">The key of the prompt template to use from configuration.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(Guid dataId, string modelName, string promptKey)
        {
            try
            {
                _logger.LogInformation("Starting AI processing for data item {DataId} using model {ModelName} and prompt '{PromptKey}'", 
                    dataId, modelName, promptKey);

                // Validate prompt exists
                if (!_promptService.HasPrompt(promptKey))
                {
                    _logger.LogError("Prompt template '{PromptKey}' not found in configuration.", promptKey);
                    return;
                }

                var promptTemplate = _promptService.GetPrompt(promptKey);

                // TODO: Replace this with your actual entity type
                // Example: var dataItem = await _repository.GetByIdAsync<YourEntity, Guid>(dataId);
                // For now, we'll use a generic approach
                
                // Step 1: Retrieve the data item from database
                // var dataItem = await _repository.GetByIdAsync<YourDataEntity, Guid>(dataId);
                // if (dataItem == null)
                // {
                //     _logger.LogWarning("Data item {DataId} not found", dataId);
                //     return;
                // }

                // Step 2: Prepare variables for prompt rendering
                // Extract actual data from your entity and map to prompt variables
                var promptVariables = new Dictionary<string, string>
                {
                    { "dataContent", $"Data ID: {dataId}" }, // Replace with actual data
                    { "requirements", "Standard analysis requirements" } // Replace with actual requirements
                    // Add more variables based on your prompt template
                };

                // Render the prompt with actual data
                var (systemPrompt, userPrompt) = _promptService.RenderPrompt(promptKey, promptVariables);

                // Step 3: Prepare the AI request
                var aiRequest = new AIGenerationRequest
                {
                    ModelName = modelName,
                    Prompt = userPrompt,
                    SystemPrompt = systemPrompt,
                    Temperature = promptTemplate!.Temperature ?? 0.7,
                    MaxTokens = promptTemplate.MaxTokens,
                    Metadata = new Dictionary<string, string>
                    {
                        { "DataId", dataId.ToString() },
                        { "PromptKey", promptKey },
                        { "ProcessedAt", DateTime.UtcNow.ToString("O") }
                    }
                };

                // Step 4: Call AI service
                var response = await _aiService.GenerateAsync(aiRequest);

                if (!response.Success)
                {
                    _logger.LogError("AI generation failed for data item {DataId}: {Error}", 
                        dataId, response.ErrorMessage);
                    
                    // TODO: Update data item status to "Failed"
                    // dataItem.ProcessingStatus = "Failed";
                    // dataItem.ErrorMessage = response.ErrorMessage;
                    // await _repository.UpdateAsync<YourDataEntity, Guid>(dataItem);
                    // await _repository.SaveChangesAsync();
                    return;
                }

                _logger.LogInformation("AI processing completed for data item {DataId} in {Duration}ms", 
                    dataId, response.DurationMs);

                // Step 5: Update the data item with results
                // dataItem.ProcessedResult = response.GeneratedText;
                // dataItem.ProcessingStatus = "Completed";
                // dataItem.ProcessedAt = DateTime.UtcNow;
                // dataItem.TokensUsed = response.TokensGenerated;
                // await _repository.UpdateAsync<YourDataEntity, Guid>(dataItem);
                // await _repository.SaveChangesAsync();

                _logger.LogInformation("Successfully updated data item {DataId} with AI results", dataId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing data item {DataId}", dataId);
                
                // TODO: Update status to failed
                throw; // Re-throw to let Hangfire handle retries
            }
        }
    }
}
