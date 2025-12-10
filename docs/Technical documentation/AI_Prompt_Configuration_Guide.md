# AI Prompt Configuration System

## Overview

The Diquis platform includes a centralized prompt configuration system that allows you to manage AI prompts in `Diquis.AI\appsettings.AI.json`. This approach provides:

- **Self-Contained AI Module**: All AI configuration lives in the Diquis.AI project
- **Centralized Management**: All prompts in one configuration file
- **Easy Updates**: Modify prompts without recompiling code
- **Template Variables**: Use `{{variable}}` placeholders for dynamic content
- **Per-Prompt Settings**: Configure temperature and max tokens per prompt
- **Type Safety**: Strongly-typed configuration classes

## Configuration Structure

### Location
**Prompts are configured in: `Diquis.AI\appsettings.AI.json`**

This file is automatically loaded by the `AddAIServices()` extension method from the Diquis.AI assembly directory, making the AI module completely self-contained.

### Format
```json
{
  "AI": {
    "Prompts": {
      "PromptKeyName": {
        "Description": "What this prompt is used for",
        "SystemPrompt": "Defines the AI's role and behavior",
        "UserPrompt": "The actual prompt with {{placeholders}}",
        "Temperature": 0.7,
        "MaxTokens": 500
      }
    }
  }
}
```

## Architecture

### Self-Contained Design
The AI configuration is loaded automatically when you call `AddAIServices()`:

```csharp
// In Program.cs (WebApi or BackgroundJobs)
builder.Services.AddAIServices(builder.Configuration);
```

The extension method:
1. Locates the `Diquis.AI` assembly directory
2. Loads `appsettings.AI.json` from that directory
3. Binds the configuration to `AIConfiguration`
4. Registers all AI services

**No manual configuration loading required!**

## Using Prompts in Your Code

### 1. Inject the IPromptService

```csharp
public class YourService
{
    private readonly IPromptService _promptService;
    private readonly IAIGenerationService _aiService;

    public YourService(
        IPromptService promptService,
        IAIGenerationService aiService)
    {
        _promptService = promptService;
        _aiService = aiService;
    }
}
```

### 2. Render a Prompt with Variables

```csharp
// Define variables to replace {{placeholders}}
var variables = new Dictionary<string, string>
{
    { "playerName", player.FirstName },
    { "parentName", parent.FirstName },
    { "ownerName", owner.FirstName }
};

// Render the prompt
var (systemPrompt, userPrompt) = _promptService.RenderPrompt("PlayerChurnPrediction", variables);
```

### 3. Get Prompt Configuration

```csharp
// Get the full prompt template including temperature and max tokens
var promptTemplate = _promptService.GetPrompt("PlayerChurnPrediction");

var request = new AIGenerationRequest
{
    ModelName = "llama2",
    Prompt = userPrompt,
    SystemPrompt = systemPrompt,
    Temperature = promptTemplate?.Temperature ?? 0.7,
    MaxTokens = promptTemplate?.MaxTokens ?? 500
};

var response = await _aiService.GenerateAsync(request);
```

### 4. Check if Prompt Exists

```csharp
if (!_promptService.HasPrompt("MyPromptKey"))
{
    _logger.LogError("Prompt template 'MyPromptKey' not found");
    return;
}
```

## Built-in Prompt Templates

The following prompt templates are pre-configured:

### TestOllama
- **Purpose**: Generic test prompt with custom input
- **Variables**: `{{prompt}}`
- **Use Case**: Testing Ollama integration

### CleanArchitecture
- **Purpose**: Default test prompt about architecture
- **Variables**: None
- **Use Case**: Verifying AI responses

### PlayerChurnPrediction
- **Purpose**: Generate re-engagement scripts for at-risk players
- **Variables**: `{{parentName}}`, `{{playerName}}`, `{{ownerName}}`
- **Use Case**: Retention management

### CashFlowInsights
- **Purpose**: Analyze cash flow forecast data
- **Variables**: `{{forecastData}}`
- **Use Case**: Financial analytics

### TrainingSessionPlan
- **Purpose**: Generate structured training plans
- **Variables**: `{{duration}}`, `{{ageGroup}}`, `{{focusArea}}`, `{{availableEquipment}}`
- **Use Case**: Coach assistance

### PlayerMatchFeedback
- **Purpose**: Generate personalized post-match feedback
- **Variables**: `{{playerName}}`, `{{matchDetails}}`, `{{metrics}}`, `{{improvements}}`
- **Use Case**: Parent communication

### BatchDataProcessing
- **Purpose**: Generic data analysis
- **Variables**: `{{dataContent}}`, `{{requirements}}`
- **Use Case**: Background job processing

## Adding New Prompts

### Step 1: Add to Configuration

Edit `appsettings.AI.json`:

```json
{
  "AI": {
    "Prompts": {
      "MyNewPrompt": {
        "Description": "Describe what this prompt does",
        "SystemPrompt": "You are an expert in...",
        "UserPrompt": "Analyze this {{dataType}} and provide {{outputFormat}}",
        "Temperature": 0.6,
        "MaxTokens": 400
      }
    }
  }
}
```

### Step 2: Use in Your Service

```csharp
public async Task<string> MyFeatureAsync(string dataType, string outputFormat)
{
    var variables = new Dictionary<string, string>
    {
        { "dataType", dataType },
        { "outputFormat", outputFormat }
    };

    var (systemPrompt, userPrompt) = _promptService.RenderPrompt("MyNewPrompt", variables);
    var template = _promptService.GetPrompt("MyNewPrompt");

    var request = new AIGenerationRequest
    {
        ModelName = "llama2",
        Prompt = userPrompt,
        SystemPrompt = systemPrompt,
        Temperature = template?.Temperature ?? 0.6,
        MaxTokens = template?.MaxTokens ?? 400
    };

    var response = await _aiService.GenerateAsync(request);
    return response.GeneratedText;
}
```

## Best Practices

### 1. Use Descriptive Keys
- ? `PlayerChurnPrediction`
- ? `Prompt1`

### 2. Document Variables
Always list required variables in the Description field:
```json
{
  "Description": "Generate feedback. Variables: playerName, matchDetails, metrics"
}
```

### 3. Set Appropriate Temperatures
- **Creative tasks** (0.7-0.9): Writing scripts, generating ideas
- **Analytical tasks** (0.3-0.6): Data analysis, summaries
- **Factual tasks** (0.1-0.3): Extracting information, classifications

### 4. Configure Max Tokens
Consider the expected output length:
- Short responses: 200-300 tokens
- Medium responses: 400-600 tokens
- Long responses: 800-1000 tokens

### 5. Use Clear System Prompts
The system prompt defines the AI's behavior. Be specific:
```json
{
  "SystemPrompt": "You are a professional youth football coach assistant. Generate personalized, encouraging feedback messages for parents about their child's performance. Keep responses professional, specific, and actionable."
}
```

## Variable Substitution

Variables use the `{{variableName}}` syntax. The prompt service will:
1. Find all `{{variableName}}` placeholders
2. Replace them with values from your dictionary
3. Leave unreplaced placeholders as-is (for debugging)

Example:
```json
{
  "UserPrompt": "Generate feedback for {{playerName}} regarding {{matchType}}."
}
```

```csharp
var variables = new Dictionary<string, string>
{
    { "playerName", "Alex" },
    { "matchType", "Championship Final" }
};

// Result: "Generate feedback for Alex regarding Championship Final."
```

## Testing Prompts

Use the test endpoint to verify your prompts:

```bash
GET /api/test/ollama?prompt=Test my custom prompt&model=llama2
```

Or update the TestOllamaJob to use your new prompt key.

## Troubleshooting

### Prompt Not Found
**Error**: `Prompt template 'XYZ' not found in configuration`

**Solution**: 
1. Check spelling in `appsettings.AI.json`
2. Ensure the configuration file is being loaded
3. Restart the application

### Variables Not Replaced
**Issue**: Seeing `{{variable}}` in output

**Solution**:
1. Check variable names match exactly (case-sensitive)
2. Ensure you're passing the variables dictionary
3. Verify the dictionary contains all required keys

### Temperature/MaxTokens Not Applied
**Issue**: Using default values instead of configured ones

**Solution**:
```csharp
// Get the template to access configured values
var template = _promptService.GetPrompt("YourPromptKey");

var request = new AIGenerationRequest
{
    Temperature = template?.Temperature ?? 0.7, // Use configured value
    MaxTokens = template?.MaxTokens ?? 500
};
```

## Migration Guide

### Before (Hardcoded Prompts)
```csharp
string systemPrompt = $"You are an expert coach...";
string userPrompt = $"Generate feedback for {playerName}...";

var request = new AIGenerationRequest
{
    Prompt = userPrompt,
    SystemPrompt = systemPrompt,
    Temperature = 0.7
};
```

### After (Configuration-Based)
```csharp
var variables = new Dictionary<string, string>
{
    { "playerName", playerName }
};

var (systemPrompt, userPrompt) = _promptService.RenderPrompt("PlayerFeedback", variables);
var template = _promptService.GetPrompt("PlayerFeedback");

var request = new AIGenerationRequest
{
    Prompt = userPrompt,
    SystemPrompt = systemPrompt,
    Temperature = template?.Temperature ?? 0.7
};
```

## Environment-Specific Prompts

You can override prompts per environment:

### appsettings.Development.AI.json
```json
{
  "AI": {
    "Prompts": {
      "TestOllama": {
        "UserPrompt": "DEVELOPMENT MODE: {{prompt}}"
      }
    }
  }
}
```

### appsettings.Production.AI.json
```json
{
  "AI": {
    "Prompts": {
      "TestOllama": {
        "UserPrompt": "{{prompt}}"
      }
    }
  }
}
