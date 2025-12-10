namespace Diquis.Application.Common.AI
{
    /// <summary>
    /// Configuration for a custom AI model.
    /// </summary>
    public class AIModelConfig
    {
        /// <summary>
        /// Gets or sets the unique name for this custom model.
        /// </summary>
        public string ModelName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the base model to use (e.g., "llama2", "mistral", "codellama").
        /// </summary>
        public string BaseModel { get; set; } = null!;

        /// <summary>
        /// Gets or sets the system prompt that defines the model's behavior.
        /// </summary>
        public string SystemPrompt { get; set; } = null!;

        /// <summary>
        /// Gets or sets the temperature setting (0.0 to 1.0).
        /// Lower values make the output more focused and deterministic.
        /// </summary>
        public double Temperature { get; set; } = 0.7;

        /// <summary>
        /// Gets or sets the top_p (nucleus sampling) parameter.
        /// </summary>
        public double? TopP { get; set; }

        /// <summary>
        /// Gets or sets the top_k parameter for token selection.
        /// </summary>
        public int? TopK { get; set; }

        /// <summary>
        /// Gets or sets the repeat penalty to prevent repetitive text.
        /// </summary>
        public double? RepeatPenalty { get; set; }

        /// <summary>
        /// Gets or sets the context window size.
        /// </summary>
        public int? ContextSize { get; set; }

        /// <summary>
        /// Gets or sets additional parameters for the model.
        /// </summary>
        public Dictionary<string, object>? AdditionalParameters { get; set; }

        /// <summary>
        /// Generates the Modelfile content for Ollama.
        /// </summary>
        public string GenerateModelfile()
        {
            var modelfile = new System.Text.StringBuilder();
            modelfile.AppendLine($"FROM {BaseModel}");
            modelfile.AppendLine();
            
            // System prompt
            if (!string.IsNullOrWhiteSpace(SystemPrompt))
            {
                var escapedPrompt = SystemPrompt.Replace("\"", "\\\"");
                modelfile.AppendLine($"SYSTEM \"{escapedPrompt}\"");
                modelfile.AppendLine();
            }

            // Parameters
            modelfile.AppendLine($"PARAMETER temperature {Temperature}");
            
            if (TopP.HasValue)
                modelfile.AppendLine($"PARAMETER top_p {TopP.Value}");
            
            if (TopK.HasValue)
                modelfile.AppendLine($"PARAMETER top_k {TopK.Value}");
            
            if (RepeatPenalty.HasValue)
                modelfile.AppendLine($"PARAMETER repeat_penalty {RepeatPenalty.Value}");
            
            if (ContextSize.HasValue)
                modelfile.AppendLine($"PARAMETER num_ctx {ContextSize.Value}");

            // Additional parameters
            if (AdditionalParameters != null)
            {
                foreach (var param in AdditionalParameters)
                {
                    modelfile.AppendLine($"PARAMETER {param.Key} {param.Value}");
                }
            }

            return modelfile.ToString();
        }
    }
}
