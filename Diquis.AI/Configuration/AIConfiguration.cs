namespace Diquis.AI.Configuration
{
    /// <summary>
    /// Configuration options for AI services.
    /// </summary>
    public class AIConfiguration
    {
        /// <summary>
        /// Configuration section name in appsettings.json.
        /// </summary>
        public const string SectionName = "AI";

        /// <summary>
        /// Gets or sets the Ollama base URL.
        /// </summary>
        public string OllamaBaseUrl { get; set; } = "http://127.0.0.1:11434";

        /// <summary>
        /// Gets or sets the default model to use.
        /// </summary>
        public string DefaultModel { get; set; } = "llama2";

        /// <summary>
        /// Gets or sets the request timeout in seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 300;

        /// <summary>
        /// Gets or sets whether to enable verbose logging.
        /// </summary>
        public bool EnableVerboseLogging { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum retry attempts for failed requests.
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Gets or sets the retry delay in milliseconds.
        /// </summary>
        public int RetryDelayMs { get; set; } = 1000;

        /// <summary>
        /// Gets or sets fallback provider configuration (optional).
        /// </summary>
        public FallbackProviderConfiguration? Fallback { get; set; }
    }

    /// <summary>
    /// Configuration for fallback AI providers (e.g., OpenAI).
    /// </summary>
    public class FallbackProviderConfiguration
    {
        /// <summary>
        /// Gets or sets whether fallback is enabled.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the provider name (e.g., "OpenAI", "Azure").
        /// </summary>
        public string Provider { get; set; } = "OpenAI";

        /// <summary>
        /// Gets or sets the API key for the fallback provider.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the endpoint URL for the fallback provider.
        /// </summary>
        public string? Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the default model for the fallback provider.
        /// </summary>
        public string? DefaultModel { get; set; }
    }
}
