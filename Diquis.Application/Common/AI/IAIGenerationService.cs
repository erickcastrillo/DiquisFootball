using Diquis.Application.Common.Marker;

namespace Diquis.Application.Common.AI
{
    /// <summary>
    /// Service interface for AI generation operations using local LLM inference.
    /// </summary>
    public interface IAIGenerationService : IScopedService
    {
        /// <summary>
        /// Creates a custom model using a Modelfile configuration.
        /// </summary>
        /// <param name="modelName">The name for the custom model.</param>
        /// <param name="baseModel">The base model to use (e.g., "llama2", "mistral").</param>
        /// <param name="systemPrompt">The system prompt to configure the model's behavior.</param>
        /// <param name="temperature">The temperature setting for generation randomness (0.0 to 1.0).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if model creation was successful, false otherwise.</returns>
        Task<bool> CreateCustomModelAsync(
            string modelName,
            string baseModel,
            string systemPrompt,
            double temperature = 0.7,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates text using a specified model.
        /// </summary>
        /// <param name="request">The AI generation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The generated text response.</returns>
        Task<AIGenerationResponse> GenerateAsync(
            AIGenerationRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates text with streaming support.
        /// </summary>
        /// <param name="request">The AI generation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An async enumerable of text chunks.</returns>
        IAsyncEnumerable<string> GenerateStreamAsync(
            AIGenerationRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a model exists in the local Ollama instance.
        /// </summary>
        /// <param name="modelName">The model name to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the model exists, false otherwise.</returns>
        Task<bool> ModelExistsAsync(
            string modelName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists all available models.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of available model names.</returns>
        Task<IEnumerable<string>> ListModelsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a custom model.
        /// </summary>
        /// <param name="modelName">The model name to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if deletion was successful, false otherwise.</returns>
        Task<bool> DeleteModelAsync(
            string modelName,
            CancellationToken cancellationToken = default);
    }
}
