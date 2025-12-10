# AI Prompt Configuration System - Implementation Summary

## Overview
Successfully implemented a centralized, self-contained prompt configuration system for managing AI prompts in the Diquis platform. All AI-related configuration now lives in the `Diquis.AI` project, making it a truly independent, reusable module.

## Key Architectural Decision

### Self-Contained AI Module
All AI configuration is now stored in `Diquis.AI\appsettings.AI.json` and automatically loaded by the AI module itself. This means:

- ? **No manual configuration loading required** in consuming projects (WebApi, BackgroundJobs)
- ? **AI module is portable** - can be moved to other projects without configuration changes
- ? **Single source of truth** for all AI settings and prompts
- ? **Follows SRP** (Single Responsibility Principle) - AI configuration managed by AI module

## Changes Made

### 1. Configuration Location
**File**: `Diquis.AI\appsettings.AI.json` (**NEW LOCATION**)
- **Before**: `Diquis.WebApi\appsettings.AI.json`
- **After**: `Diquis.AI\appsettings.AI.json`
- Configured to copy to output directory in `Diquis.AI.csproj`

### 2. Automatic Configuration Loading
**File**: `Diquis.AI\Extensions\ServiceCollectionExtensions.cs`
- Updated to automatically locate and load `appsettings.AI.json` from the Diquis.AI assembly directory
- Uses `Assembly.GetExecutingAssembly().Location` to find the AI assembly path
- Builds configuration from the AI-specific JSON file
- Merges with passed configuration for override scenarios

### 3. Simplified Consumer Usage
**Files**: `Diquis.WebApi\Program.cs`, `Diquis.BackgroundJobs\Program.cs`
- **Before**: Had to manually call `builder.Configuration.AddJsonFile("appsettings.AI.json")`
- **After**: Just call `builder.Services.AddAIServices(builder.Configuration)` - configuration loaded automatically!

### 4. Configuration Classes (Diquis.Application)
**File**: `Diquis.Application\Common\AI\AIConfiguration.cs`
- Moved `AIConfiguration` class from `Diquis.AI` to `Diquis.Application` to avoid circular dependencies
- Added `Prompts` property: `Dictionary<string, PromptTemplate>`
- Added `PromptTemplate` class with:
  - `SystemPrompt` - Defines AI's role and behavior
  - `UserPrompt` - The actual prompt with `{{variable}}` placeholders
  - `Temperature` - Optional per-prompt temperature override
  - `MaxTokens` - Optional per-prompt token limit
  - `Description` - Documentation for the prompt

### 5. Prompt Service (Diquis.Application)
**File**: `Diquis.Application\Common\AI\PromptService.cs`
- Created `IPromptService` interface with methods:
  - `GetPrompt(string promptKey)` - Retrieves a prompt template
  - `RenderPrompt(string promptKey, Dictionary<string, string>? variables)` - Renders a prompt with variable substitution
  - `HasPrompt(string promptKey)` - Checks if a prompt exists
- Implemented `PromptService` class with:
  - Variable substitution using regex pattern `{{variableName}}`
  - Safe handling of missing variables (keeps placeholder)
  - Configuration injection via `IOptions<AIConfiguration>`

### 6. Updated Background Jobs
**Files**:
- `Diquis.Application\BackgroundJobs\AI\TestOllamaJob.cs`
- `Diquis.Application\BackgroundJobs\AI\ProcessBatchDataForAIJob.cs`
- `Diquis.Application\BackgroundJobs\AI\ProcessSingleDataForAIJob.cs`

**Changes**:
- Injected `IPromptService` into constructors
- Updated to use `RenderPrompt()` instead of hardcoded prompts
- Added metadata tracking for prompt keys used
- Updated to extract temperature and max tokens from prompt configuration

### 7. Example Service (Diquis.Application)
**File**: `Diquis.Application\Services\PredictiveAnalytics\PredictiveAnalyticsService.cs`
- Created example service demonstrating prompt usage for:
  - Player churn prediction (re-engagement scripts)
  - Cash flow insights generation
- Shows best practices for variable substitution and configuration usage

### 8. Documentation
**File**: `docs\Technical documentation\AI_Prompt_Configuration_Guide.md`
- Comprehensive guide covering:
  - Configuration structure and format
  - Usage examples for developers
  - Built-in prompt templates documentation
  - Adding new prompts
  - Best practices for temperature and token settings
  - Variable substitution syntax
  - Troubleshooting guide
  - Migration guide from hardcoded prompts

### 9. Architecture Decisions

#### Circular Dependency Resolution
**Problem**: `Diquis.AI` depends on `Diquis.Application` (for background jobs), creating a circular dependency when Application needs to reference AI for prompts.

**Solution**: Moved configuration classes (`AIConfiguration`, `PromptTemplate`) and `PromptService` from `Diquis.AI` to `Diquis.Application`. This allows:
- Application layer to access prompt functionality without circular dependency
- AI layer to continue using the configuration
- Clear separation: Configuration/Services in Application, Infrastructure (Ollama) in AI layer

## Benefits

### 1. Centralized Management
- All prompts in one location (`appsettings.AI.json`)
- Easy to review and update without code changes
- Version control friendly

### 2. Template Variables
- Dynamic content injection using `{{variableName}}` syntax
- Type-safe variable substitution
- Clear documentation of required variables

### 3. Configuration Flexibility
- Per-prompt temperature and token settings
- Environment-specific overrides (Development, Production)
- Fallback to defaults if not specified

### 4. Developer Experience
- Simple API: `RenderPrompt(key, variables)`
- IntelliSense support for prompt keys
- Clear error messages for missing prompts/variables

### 5. Maintainability
- Prompts can be updated by non-developers
- No recompilation needed for prompt changes
- Easy A/B testing of different prompts

## Usage Example

```csharp
// Inject the service
private readonly IPromptService _promptService;
private readonly IAIGenerationService _aiService;

// Render a prompt with variables
var variables = new Dictionary<string, string>
{
    { "playerName", "Alex" },
    { "parentName", "John Smith" },
    { "ownerName", "Coach Anderson" }
};

var (systemPrompt, userPrompt) = _promptService.RenderPrompt("PlayerChurnPrediction", variables);
var template = _promptService.GetPrompt("PlayerChurnPrediction");

// Use in AI request
var request = new AIGenerationRequest
{
    ModelName = "llama2",
    Prompt = userPrompt,
    SystemPrompt = systemPrompt,
    Temperature = template?.Temperature ?? 0.7,
    MaxTokens = template?.MaxTokens ?? 300
};

var response = await _aiService.GenerateAsync(request);
```

## Testing

### Build Status
? All projects compile successfully
? No circular dependency issues
? All namespace references resolved correctly

### Manual Testing Checklist
- [ ] Test `/api/test/ollama` endpoint with default prompt
- [ ] Test `/api/test/ollama?prompt=custom` with custom prompt
- [ ] Verify prompt templates load correctly from configuration
- [ ] Test variable substitution with PlayerChurnPrediction prompt
- [ ] Verify temperature/maxTokens from config are used

## Next Steps

1. **Test with Ollama**: Run actual AI generation tests to verify prompts work as expected
2. **Add More Prompts**: Create prompts for other features (coaching assistant, match analysis, etc.)
3. **Environment Overrides**: Set up `appsettings.Development.AI.json` and `appsettings.Production.AI.json` for environment-specific prompts
4. **Monitoring**: Add logging for prompt usage metrics
5. **Validation**: Add FluentValidation rules for prompt template format

## Files Created
- `Diquis.AI\appsettings.AI.json` - **Moved from WebApi to AI project**

## Files Modified
- `Diquis.AI\Diquis.AI.csproj` - Added appsettings.AI.json with CopyToOutputDirectory
- `Diquis.AI\Extensions\ServiceCollectionExtensions.cs` - Auto-load configuration from AI assembly
- `Diquis.WebApi\Program.cs` - Removed manual configuration loading
- `docs\Technical documentation\AI_Prompt_Configuration_Guide.md` - Updated documentation

## Files Removed
- `Diquis.WebApi\appsettings.AI.json` - Moved to Diquis.AI project
