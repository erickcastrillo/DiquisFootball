# Task Context
Implement the `AIChatService` which orchestrates the conversation. The critical part is constructing the system prompt by injecting content from `KnowledgeBaseArticle`s to constrain the LLM.

# Core References
- **Plan:** [19 - Implementation_Plan_AISalesAgent.md](./19%20-%20Implementation_Plan_AISalesAgent.md)
- **Tech Guide:** [AI_Sales_Agent_TechnicalGuide.md](../../Technical%20documentation/AI_Sales_Agent_TechnicalGuide/AI_Sales_Agent_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `AIChatService.cs`:**
    *   Path: `Diquis.Application/Services/AIChat/AIChatService.cs`.
    *   Inject `IBaseRepositoryAsync<KnowledgeBaseArticle>`, `IBaseRepositoryAsync<ChatSession>`, `IOpenAIService`.
2.  **Implement `RespondAsync`:**
    *   Get/Create Session.
    *   Retrieve enabled Knowledge Base articles.
    *   Construct System Prompt:
        *   Define Persona (Expert AI Sales Assistant).
        *   **Constraint:** "Responses MUST be strictly based on the KNOWLEDGE BASE."
        *   Inject Knowledge Base content.
        *   Set Language instruction based on `DetectedLanguage`.
    *   Call LLM Service.
    *   Update Transcript and Save Session.
3.  **Unit Test:**
    *   Create `Diquis.Application.Tests/AIChat/ContextInjectionTests.cs`.
    *   Verify prompt contains knowledge base content.

# Acceptance Criteria
- [ ] `AIChatService` implemented.
- [ ] System prompt correctly constructs with constraints and context.
- [ ] Unit tests pass.
