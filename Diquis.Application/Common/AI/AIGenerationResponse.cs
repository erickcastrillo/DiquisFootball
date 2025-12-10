namespace Diquis.Application.Common.AI
{
    /// <summary>
    /// Represents the response from an AI generation request.
    /// </summary>
    public class AIGenerationResponse
    {
        /// <summary>
        /// Gets or sets the generated text.
        /// </summary>
        public string GeneratedText { get; set; } = null!;

        /// <summary>
        /// Gets or sets the model used for generation.
        /// </summary>
        public string ModelName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the number of tokens generated.
        /// </summary>
        public int? TokensGenerated { get; set; }

        /// <summary>
        /// Gets or sets the time taken for generation in milliseconds.
        /// </summary>
        public long DurationMs { get; set; }

        /// <summary>
        /// Gets or sets whether the generation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error message if generation failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets additional metadata from the generation.
        /// </summary>
        public Dictionary<string, object>? Metadata { get; set; }

        /// <summary>
        /// Creates a successful response.
        /// </summary>
        public static AIGenerationResponse Successful(string generatedText, string modelName, long durationMs, int? tokensGenerated = null)
        {
            return new AIGenerationResponse
            {
                Success = true,
                GeneratedText = generatedText,
                ModelName = modelName,
                DurationMs = durationMs,
                TokensGenerated = tokensGenerated
            };
        }

        /// <summary>
        /// Creates a failed response.
        /// </summary>
        public static AIGenerationResponse Failed(string errorMessage, string modelName)
        {
            return new AIGenerationResponse
            {
                Success = false,
                ErrorMessage = errorMessage,
                ModelName = modelName,
                GeneratedText = string.Empty
            };
        }
    }
}
