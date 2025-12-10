namespace Diquis.Application.Common.AI
{
    /// <summary>
    /// Represents a request for AI text generation.
    /// </summary>
    public class AIGenerationRequest
    {
        /// <summary>
        /// Gets or sets the model to use for generation.
        /// </summary>
        public string ModelName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user prompt/input.
        /// </summary>
        public string Prompt { get; set; } = null!;

        /// <summary>
        /// Gets or sets the system prompt (optional, overrides model's default).
        /// </summary>
        public string? SystemPrompt { get; set; }

        /// <summary>
        /// Gets or sets the temperature for generation (0.0 to 1.0).
        /// </summary>
        public double Temperature { get; set; } = 0.7;

        /// <summary>
        /// Gets or sets the maximum number of tokens to generate.
        /// </summary>
        public int? MaxTokens { get; set; }

        /// <summary>
        /// Gets or sets additional context for the generation (e.g., conversation history).
        /// </summary>
        public string? Context { get; set; }

        /// <summary>
        /// Gets or sets metadata for tracking purposes.
        /// </summary>
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
