# AI Prompt Configuration - Quick Reference

## Configuration Location

**All AI configuration is stored in:** `Diquis.AI\appsettings.AI.json`

This file is automatically loaded when you call `AddAIServices()` - no manual configuration required!

## Adding a New Prompt

### 1. Edit appsettings.AI.json (in Diquis.AI project)
```json
{
  "AI": {
    "Prompts": {
      "YourPromptName": {
        "Description": "What this prompt does",
        "SystemPrompt": "You are an expert in...",
        "UserPrompt": "Analyze {{data}} and provide {{format}}",
        "Temperature": 0.7,
        "MaxTokens": 500
      }
    }
  }
}
```

### 2. Use in Your Service
```csharp
public class YourService
{
    private readonly IPromptService _promptService;
    private readonly IAIGenerationService _aiService;
    
    public async Task<string> YourMethodAsync()
    {
        // 1. Prepare variables
        var variables = new Dictionary<string, string>
        {
            { "data", yourData },
            { "format", "JSON" }
        };
        
        // 2. Render prompt
        var (systemPrompt, userPrompt) = _promptService.RenderPrompt("YourPromptName", variables);
        var template = _promptService.GetPrompt("YourPromptName");
        
        // 3. Create AI request
        var request = new AIGenerationRequest
        {
            ModelName = "llama2",
            Prompt = userPrompt,
            SystemPrompt = systemPrompt,
            Temperature = template?.Temperature ?? 0.7,
            MaxTokens = template?.MaxTokens ?? 500
        };
        
        // 4. Generate
        var response = await _aiService.GenerateAsync(request);
        return response.GeneratedText;
    }
}
```

## Variable Substitution

### Syntax
Use `{{variableName}}` in your prompts:
```json
{
  "UserPrompt": "Generate feedback for {{playerName}} about {{topic}}"
}
```

### Code
```csharp
var variables = new Dictionary<string, string>
{
    { "playerName", "Alex" },
    { "topic", "teamwork" }
};
var (system, user) = _promptService.RenderPrompt("MyPrompt", variables);
// Result: "Generate feedback for Alex about teamwork"
```

## Built-in Prompts

| Key | Purpose | Variables |
|-----|---------|-----------|
| `TestOllama` | Test with custom prompt | `{{prompt}}` |
| `CleanArchitecture` | Architecture explanation | None |
| `PlayerChurnPrediction` | Re-engagement scripts | `{{parentName}}`, `{{playerName}}`, `{{ownerName}}` |
| `CashFlowInsights` | Financial analysis | `{{forecastData}}` |
| `TrainingSessionPlan` | Training plans | `{{duration}}`, `{{ageGroup}}`, `{{focusArea}}`, `{{availableEquipment}}` |
| `PlayerMatchFeedback` | Post-match feedback | `{{playerName}}`, `{{matchDetails}}`, `{{metrics}}`, `{{improvements}}` |
| `BatchDataProcessing` | Generic processing | `{{dataContent}}`, `{{requirements}}` |

## IPromptService Methods

```csharp
// Get a template
PromptTemplate? template = _promptService.GetPrompt("PromptKey");

// Render with variables
(string system, string user) = _promptService.RenderPrompt("PromptKey", variables);

// Check existence
bool exists = _promptService.HasPrompt("PromptKey");
```

## Temperature Guide

| Value | Use Case | Examples |
|-------|----------|----------|
| 0.1-0.3 | Factual, deterministic | Data extraction, classification |
| 0.4-0.6 | Balanced analysis | Financial insights, summaries |
| 0.7-0.8 | Creative, conversational | Feedback messages, scripts |
| 0.9-1.0 | Highly creative | Brainstorming, storytelling |

## Common Patterns

### Pattern 1: Simple Prompt (No Variables)
```json
{
  "TestPrompt": {
    "SystemPrompt": "You are helpful.",
    "UserPrompt": "Explain Clean Architecture.",
    "Temperature": 0.7
  }
}
```

### Pattern 2: Template with Variables
```json
{
  "FeedbackPrompt": {
    "SystemPrompt": "You are a coach.",
    "UserPrompt": "Give feedback to {{player}} about {{topic}}.",
    "Temperature": 0.8
  }
}
```

### Pattern 3: Multi-line with Constraints
```json
{
  "AnalysisPrompt": {
    "SystemPrompt": "You are an analyst.\n\n**CONSTRAINTS:**\n- Be concise\n- Use data only",
    "UserPrompt": "Analyze:\n{{data}}\n\nFormat: {{format}}",
    "Temperature": 0.5,
    "MaxTokens": 600
  }
}
```

## Troubleshooting

### Prompt Not Found
```
Error: Prompt template 'XYZ' not found in configuration
```
**Fix**: Check spelling in `appsettings.AI.json` and restart app.

### Variables Not Replaced
```
Output contains: "Hello {{name}}"
```
**Fix**: Ensure variable name matches exactly (case-sensitive).

### Wrong Temperature Used
```csharp
// ? Wrong - not using configured value
var request = new AIGenerationRequest { Temperature = 0.7 };

// ? Correct - use configured value
var template = _promptService.GetPrompt("MyPrompt");
var request = new AIGenerationRequest { 
    Temperature = template?.Temperature ?? 0.7 
};
```

## Testing Your Prompts

### Via API
```bash
GET /api/test/ollama?prompt=Your test prompt&model=llama2
```

### In Code
```csharp
[Test]
public void TestPromptRendering()
{
    var vars = new Dictionary<string, string> { { "name", "Test" } };
    var (system, user) = _promptService.RenderPrompt("MyPrompt", vars);
    
    Assert.That(user, Does.Contain("Test"));
    Assert.That(user, Does.Not.Contain("{{"));
}
```

## Best Practices

1. **? DO**: Use descriptive prompt keys (`PlayerChurnPrediction`, not `Prompt1`)
2. **? DO**: Document required variables in Description
3. **? DO**: Set appropriate temperature per prompt
4. **? DO**: Use constraints in system prompts
5. **? DON'T**: Hardcode prompts in code
6. **? DON'T**: Use generic keys (`Test`, `Temp`, etc.)
7. **? DON'T**: Forget to handle missing variables
8. **? DON'T**: Set temperature too high for factual tasks

## Quick Links

- **Configuration File**: `Diquis.AI\appsettings.AI.json` ?
- Full Guide: `docs\Technical documentation\AI_Prompt_Configuration_Guide.md`
- Implementation Summary: `docs\Technical documentation\AI_Prompt_Configuration_Implementation_Summary.md`
- Service Code: `Diquis.Application\Common\AI\PromptService.cs`
- Auto-loader: `Diquis.AI\Extensions\ServiceCollectionExtensions.cs`
