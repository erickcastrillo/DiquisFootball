using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Diquis.Application.Common.AI
{
    /// <summary>
    /// Service for managing and rendering AI prompt templates.
    /// </summary>
    public interface IPromptService
    {
        /// <summary>
        /// Gets a prompt template by key.
        /// </summary>
        /// <param name="promptKey">The key of the prompt template.</param>
        /// <returns>The prompt template, or null if not found.</returns>
        PromptTemplate? GetPrompt(string promptKey);

        /// <summary>
        /// Renders a prompt template by replacing placeholders with actual values.
        /// </summary>
        /// <param name="promptKey">The key of the prompt template.</param>
        /// <param name="variables">Dictionary of variable names and their values.</param>
        /// <returns>Rendered system and user prompts.</returns>
        (string SystemPrompt, string UserPrompt) RenderPrompt(string promptKey, Dictionary<string, string>? variables = null);

        /// <summary>
        /// Checks if a prompt exists.
        /// </summary>
        /// <param name="promptKey">The key to check.</param>
        /// <returns>True if the prompt exists, false otherwise.</returns>
        bool HasPrompt(string promptKey);
    }

    /// <summary>
    /// Implementation of prompt management service.
    /// </summary>
    public class PromptService : IPromptService
    {
        private readonly AIConfiguration _config;

        public PromptService(IOptions<AIConfiguration> config)
        {
            _config = config.Value;
        }

        /// <inheritdoc/>
        public PromptTemplate? GetPrompt(string promptKey)
        {
            return _config.Prompts.TryGetValue(promptKey, out var prompt) ? prompt : null;
        }

        /// <inheritdoc/>
        public bool HasPrompt(string promptKey)
        {
            return _config.Prompts.ContainsKey(promptKey);
        }

        /// <inheritdoc/>
        public (string SystemPrompt, string UserPrompt) RenderPrompt(string promptKey, Dictionary<string, string>? variables = null)
        {
            if (!_config.Prompts.TryGetValue(promptKey, out var template))
            {
                throw new ArgumentException($"Prompt template '{promptKey}' not found in configuration.", nameof(promptKey));
            }

            variables ??= new Dictionary<string, string>();

            var systemPrompt = ReplaceVariables(template.SystemPrompt, variables);
            var userPrompt = ReplaceVariables(template.UserPrompt, variables);

            return (systemPrompt, userPrompt);
        }

        /// <summary>
        /// Replaces {{variable}} placeholders in a template string.
        /// </summary>
        private string ReplaceVariables(string template, Dictionary<string, string> variables)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            // Pattern to match {{variableName}}
            var pattern = @"\{\{(\w+)\}\}";
            
            return Regex.Replace(template, pattern, match =>
            {
                var variableName = match.Groups[1].Value;
                
                if (variables.TryGetValue(variableName, out var value))
                {
                    return value;
                }
                
                // If variable not found, keep the placeholder
                return match.Value;
            });
        }
    }
}
