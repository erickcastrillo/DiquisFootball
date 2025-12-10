using Diquis.Application.Common.AI;
using Microsoft.Extensions.Logging;

namespace Diquis.Application.Services.PredictiveAnalytics
{
    /// <summary>
    /// Example service demonstrating how to use configured prompts for AI-powered features.
    /// This service generates re-engagement scripts for at-risk players.
    /// </summary>
    public interface IPredictiveAnalyticsService
    {
        /// <summary>
        /// Generates a re-engagement script for contacting a parent of an at-risk player.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <returns>The generated re-engagement script.</returns>
        Task<string> GenerateReEngagementScriptAsync(Guid playerId);

        /// <summary>
        /// Generates cash flow insights from forecast data.
        /// </summary>
        /// <param name="forecastDataJson">JSON representation of forecast data.</param>
        /// <returns>AI-generated insights summary.</returns>
        Task<string> GenerateCashFlowInsightsAsync(string forecastDataJson);
    }

    /// <summary>
    /// Implementation of predictive analytics service using configured AI prompts.
    /// </summary>
    public class PredictiveAnalyticsService : IPredictiveAnalyticsService
    {
        private readonly IAIGenerationService _aiService;
        private readonly IPromptService _promptService;
        private readonly ILogger<PredictiveAnalyticsService> _logger;
        // TODO: Add your DbContext or Repository dependencies here

        public PredictiveAnalyticsService(
            IAIGenerationService aiService,
            IPromptService promptService,
            ILogger<PredictiveAnalyticsService> logger)
        {
            _aiService = aiService;
            _promptService = promptService;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<string> GenerateReEngagementScriptAsync(Guid playerId)
        {
            try
            {
                _logger.LogInformation("Generating re-engagement script for player {PlayerId}", playerId);

                // TODO: Fetch actual player and parent data from database
                // Example:
                // var player = await _context.Users.FindAsync(playerId);
                // var parent = await _context.Users.FindAsync(player.ParentId);
                // var owner = await _context.Users.FindAsync(_currentUserService.UserId);

                // For demonstration, using placeholder data
                var variables = new Dictionary<string, string>
                {
                    { "parentName", "John Smith" }, // Replace with: parent.FirstName
                    { "playerName", "Alex Smith" }, // Replace with: player.FirstName
                    { "ownerName", "Coach Anderson" } // Replace with: owner.FirstName
                };

                // Render the prompt using the configured template
                var (systemPrompt, userPrompt) = _promptService.RenderPrompt("PlayerChurnPrediction", variables);

                // Get the prompt configuration for temperature and max tokens
                var promptTemplate = _promptService.GetPrompt("PlayerChurnPrediction");

                // Create AI request
                var request = new AIGenerationRequest
                {
                    ModelName = "llama2", // Or get from configuration
                    Prompt = userPrompt,
                    SystemPrompt = systemPrompt,
                    Temperature = promptTemplate?.Temperature ?? 0.8,
                    MaxTokens = promptTemplate?.MaxTokens ?? 300,
                    Metadata = new Dictionary<string, string>
                    {
                        { "Feature", "PlayerChurnPrediction" },
                        { "PlayerId", playerId.ToString() },
                        { "GeneratedAt", DateTime.UtcNow.ToString("O") }
                    }
                };

                // Generate the script
                var response = await _aiService.GenerateAsync(request);

                if (!response.Success)
                {
                    _logger.LogError("Failed to generate re-engagement script: {Error}", response.ErrorMessage);
                    throw new InvalidOperationException($"AI generation failed: {response.ErrorMessage}");
                }

                _logger.LogInformation("Successfully generated re-engagement script for player {PlayerId} in {Duration}ms", 
                    playerId, response.DurationMs);

                return response.GeneratedText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating re-engagement script for player {PlayerId}", playerId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<string> GenerateCashFlowInsightsAsync(string forecastDataJson)
        {
            try
            {
                _logger.LogInformation("Generating cash flow insights from forecast data");

                var variables = new Dictionary<string, string>
                {
                    { "forecastData", forecastDataJson }
                };

                var (systemPrompt, userPrompt) = _promptService.RenderPrompt("CashFlowInsights", variables);
                var promptTemplate = _promptService.GetPrompt("CashFlowInsights");

                var request = new AIGenerationRequest
                {
                    ModelName = "llama2",
                    Prompt = userPrompt,
                    SystemPrompt = systemPrompt,
                    Temperature = promptTemplate?.Temperature ?? 0.5,
                    MaxTokens = promptTemplate?.MaxTokens ?? 200,
                    Metadata = new Dictionary<string, string>
                    {
                        { "Feature", "CashFlowInsights" },
                        { "GeneratedAt", DateTime.UtcNow.ToString("O") }
                    }
                };

                var response = await _aiService.GenerateAsync(request);

                if (!response.Success)
                {
                    _logger.LogError("Failed to generate cash flow insights: {Error}", response.ErrorMessage);
                    throw new InvalidOperationException($"AI generation failed: {response.ErrorMessage}");
                }

                _logger.LogInformation("Successfully generated cash flow insights in {Duration}ms", response.DurationMs);

                return response.GeneratedText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating cash flow insights");
                throw;
            }
        }
    }
}
