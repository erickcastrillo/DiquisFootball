using Diquis.Application.Common;
using Diquis.Application.Common.AI;
using Microsoft.Extensions.Logging;

namespace Diquis.Application.BackgroundJobs.AI
{
    /// <summary>
    /// Parent Hangfire job that queries pending data and enqueues child jobs for AI processing.
    /// This job runs on a schedule (e.g., every 5 minutes).
    /// </summary>
    public class ProcessBatchDataForAIJob
    {
        private readonly IRepositoryAsync _repository;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IAIGenerationService _aiService;
        private readonly ILogger<ProcessBatchDataForAIJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessBatchDataForAIJob"/> class.
        /// </summary>
        public ProcessBatchDataForAIJob(
            IRepositoryAsync repository,
            IBackgroundJobService backgroundJobService,
            IAIGenerationService aiService,
            ILogger<ProcessBatchDataForAIJob> logger)
        {
            _repository = repository;
            _backgroundJobService = backgroundJobService;
            _aiService = aiService;
            _logger = logger;
        }

        /// <summary>
        /// Executes the batch processing job.
        /// Queries the database for pending items and enqueues child jobs.
        /// </summary>
        /// <param name="modelName">The AI model to use for processing.</param>
        /// <param name="systemPrompt">The system prompt for the model.</param>
        /// <param name="batchSize">Maximum number of items to process in this batch.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(string modelName, string systemPrompt, int batchSize = 100)
        {
            try
            {
                _logger.LogInformation("Starting batch AI processing job. Model: {ModelName}, Batch Size: {BatchSize}", 
                    modelName, batchSize);

                // Step 1: Ensure the model exists or create it
                var modelExists = await _aiService.ModelExistsAsync(modelName);
                if (!modelExists)
                {
                    _logger.LogWarning("Model {ModelName} does not exist. Attempting to create...", modelName);
                    
                    // Extract base model from modelName (e.g., "custom-llama2" -> "llama2")
                    var baseModel = ExtractBaseModel(modelName);
                    
                    var created = await _aiService.CreateCustomModelAsync(
                        modelName,
                        baseModel,
                        systemPrompt,
                        temperature: 0.7);

                    if (!created)
                    {
                        _logger.LogError("Failed to create model {ModelName}. Aborting batch job.", modelName);
                        return;
                    }

                    _logger.LogInformation("Successfully created model {ModelName}", modelName);
                }

                // Step 2: Query pending data items
                // TODO: Replace with your actual entity and query logic
                // Example using a specification pattern:
                // var spec = new PendingDataForAISpec(batchSize);
                // var pendingItems = await _repository.GetListAsync<YourDataEntity, Guid>(spec);

                // For demonstration, let's assume we have a method to get pending IDs
                var pendingIds = await GetPendingDataIds(batchSize);

                if (!pendingIds.Any())
                {
                    _logger.LogInformation("No pending data items found for AI processing");
                    return;
                }

                _logger.LogInformation("Found {Count} pending data items for processing", pendingIds.Count());

                // Step 3: Enqueue child jobs for each pending item
                var enqueuedCount = 0;
                foreach (var dataId in pendingIds)
                {
                    try
                    {
#pragma warning disable CS4014 // Intentional fire-and-forget background job
                        _backgroundJobService.Enqueue(() =>
                            new ProcessSingleDataForAIJob(default!, default!, default!)
                                .ExecuteAsync(dataId, modelName, systemPrompt));
#pragma warning restore CS4014

                        enqueuedCount++;
                        
                        _logger.LogDebug("Enqueued AI processing job for data item {DataId}", dataId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to enqueue job for data item {DataId}", dataId);
                    }
                }

                _logger.LogInformation("Successfully enqueued {EnqueuedCount} AI processing jobs", enqueuedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in batch AI processing job");
                throw;
            }
        }

        /// <summary>
        /// Retrieves pending data item IDs that need AI processing.
        /// </summary>
        /// <param name="limit">Maximum number of IDs to retrieve.</param>
        /// <returns>Collection of data item IDs.</returns>
        private async Task<IEnumerable<Guid>> GetPendingDataIds(int limit)
        {
            // TODO: Implement your actual database query
            // Example:
            // var spec = new PendingDataForAISpec(limit);
            // var items = await _repository.GetListAsync<YourDataEntity, Guid>(spec);
            // return items.Select(x => x.Id);

            // Placeholder implementation
            _logger.LogWarning("GetPendingDataIds is using placeholder implementation. Implement actual database query.");
            return Enumerable.Empty<Guid>();

            /* EXAMPLE IMPLEMENTATION:
            using var dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            
            var pendingItems = await dbContext.YourDataEntities
                .Where(x => x.ProcessingStatus == "Pending" || x.ProcessingStatus == null)
                .OrderBy(x => x.CreatedOn)
                .Take(limit)
                .Select(x => x.Id)
                .ToListAsync();

            return pendingItems;
            */
        }

        /// <summary>
        /// Extracts the base model name from a custom model name.
        /// </summary>
        /// <param name="modelName">The custom model name.</param>
        /// <returns>The base model name.</returns>
        private string ExtractBaseModel(string modelName)
        {
            // Simple heuristic: if modelName contains a dash, take the part after it
            // e.g., "custom-llama2" -> "llama2"
            if (modelName.Contains('-'))
            {
                var parts = modelName.Split('-');
                return parts.Length > 1 ? parts[1] : modelName;
            }

            // Default to llama2 if we can't determine
            return "llama2";
        }
    }
}
