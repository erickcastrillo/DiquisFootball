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
        private readonly IRepositoryAsync _repository;
        private readonly ILogger<ProcessSingleDataForAIJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSingleDataForAIJob"/> class.
        /// </summary>
        public ProcessSingleDataForAIJob(
            IAIGenerationService aiService,
            IRepositoryAsync repository,
            ILogger<ProcessSingleDataForAIJob> logger)
        {
            _aiService = aiService;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Executes the AI processing for a single data item.
        /// </summary>
        /// <param name="dataId">The ID of the data item to process.</param>
        /// <param name="modelName">The AI model to use.</param>
        /// <param name="systemPrompt">The system prompt for the model.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(Guid dataId, string modelName, string systemPrompt)
        {
            try
            {
                _logger.LogInformation("Starting AI processing for data item {DataId} using model {ModelName}", 
                    dataId, modelName);

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

                // Step 2: Prepare the AI request
                var aiRequest = new AIGenerationRequest
                {
                    ModelName = modelName,
                    Prompt = $"Process this data: {dataId}", // Replace with actual data
                    SystemPrompt = systemPrompt,
                    Temperature = 0.7,
                    Metadata = new Dictionary<string, string>
                    {
                        { "DataId", dataId.ToString() },
                        { "ProcessedAt", DateTime.UtcNow.ToString("O") }
                    }
                };

                // Step 3: Call AI service
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

                // Step 4: Update the data item with results
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
