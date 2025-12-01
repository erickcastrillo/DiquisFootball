# Technical Implementation Guide: Module 16-A (AI Sales Agent)

This document provides a detailed technical guide for implementing the AI Sales Agent chatbot as outlined in the "External AI Agents (Sales & Support)" Functional Requirement Specification (FRS).

## 1. Architectural Analysis

### Domain Entities

To support the AI chatbot's functionality, two new global entities are required. They will exist outside the tenanting structure as they deal with pre-conversion, anonymous user interactions.

1.  **`KnowledgeBaseArticle`**: Represents a single piece of approved content the AI can learn from.
    *   `Title` (string, required)
    *   `Content` (string, required, text format)
    *   `SourceUrl` (string, unique, for reference, e.g., "/pricing")
    *   `IsEnabled` (bool)

2.  **`ChatSession`**: Represents a single, complete conversation with a website visitor.
    *   `VisitorId` (string, required): A unique identifier for the anonymous visitor (e.g., a GUID stored in a client-side cookie).
    *   `DetectedLanguage` (string, e.g., "en", "es"): The language detected from the user's input.
    *   `Transcript` (JSON, string): A serialized list of all messages in the conversation.
    *   `Status` (enum: `Active`, `LeadCaptured`, `Escalated`, `Abandoned`).
    *   `CreatedAt` (DateTime).

### Multi-Tenancy Scope

-   **Shared/Global (`BaseDbContext`):**
    -   `KnowledgeBaseArticle`: This is a global resource managed by system administrators. It **must not** implement `IMustHaveTenant`.
    -   `ChatSession`: These sessions occur before a user is associated with a tenant. They are global and **must not** implement `IMustHaveTenant`.

### Permissions & Authorization

| Actor | FRS Role | Policy Name | Permissions Claim | Implementation Detail |
| :--- | :--- | :--- | :--- | :--- |
| Anonymous User | `Website Visitor` | `[AllowAnonymous]` | (none) | Applied to the public-facing chat endpoint. |
| Diquis Staff | `Sales Agent` | `IsSalesAgent` | `permission:sales.leads.view` | Secures the internal controller for viewing escalated chats. |
| Diquis Admin | `super_user` | `IsSuperAdmin` | `permission:knowledgebase.manage` | Secures the `KnowledgeBaseArticlesController`. |

## 2. Scaffolding Strategy (CLI)

Execute these commands from the `Diquis.Application/Services` directory to create the required vertical slices.

1.  **Knowledge Base Management (for Admins):**
    ```bash
    dotnet new nano-service -s KnowledgeBaseArticle -p KnowledgeBaseArticles -ap Diquis
    dotnet new nano-controller -s KnowledgeBaseArticle -p KnowledgeBaseArticles -ap Diquis
    ```

2.  **Chat Session Management (Public & Internal):**
    ```bash
    dotnet new nano-service -s ChatSession -p ChatSessions -ap Diquis
    dotnet new nano-controller -s ChatSession -p ChatSessions -ap Diquis
    ```

## 3. Implementation Plan (Agile Breakdown)

### User Story: AI Knowledge Constraint & Intent Detection
**As a** Website Visitor, **I want** to get immediate, accurate answers that are relevant to Diquis, **so that** I can evaluate the product efficiently.

**Technical Tasks:**
1.  **Domain:** Create `KnowledgeBaseArticle` and `ChatSession` entities in `Diquis.Domain`.
2.  **Persistence:** Add `DbSet<KnowledgeBaseArticle>` and `DbSet<ChatSession>` to the **`BaseDbContext`** (not `ApplicationDbContext`).
    -   *Migration Command:* `add-migration -Context BaseDbContext -o Persistence/Migrations/BaseDb AddAiSalesEntities`
3.  **Application (DTOs):** In `Diquis.Application/Services/ChatSessions/DTOs`, create:
    -   `PostMessageRequest` (`string? VisitorId`, `string Text`).
    -   `PostMessageResponse` (`string VisitorId`, `string ResponseText`).
    -   `ChatMessage` (POCO for serialization into the transcript): `Sender` ("user" or "ai"), `Text`, `Timestamp`.
4.  **Application (Service):** Create a new, non-scaffolded service: `AIChatService.cs` (and `IAIChatService`). This service will be the core of the module.
    -   Inject `IBaseRepositoryAsync<KnowledgeBaseArticle>` and `IBaseRepositoryAsync<ChatSession>`.
    -   Implement `RespondAsync(PostMessageRequest request)`. This method will contain the main business logic.
    -   **Crucial:** Implement an intent detection mechanism using keywords from the FRS (`Enterprise`, `price`, etc.).
5.  **Infrastructure:**
    -   Create an `OpenAIService` (or similar for another provider) in the Infrastructure layer that takes a system prompt and user message and returns the AI's response.
6.  **API:**
    -   In `ChatSessionsController`, create a public `POST /api/chat/messages` endpoint marked with `[AllowAnonymous]`. This will call `AIChatService.RespondAsync`.
    -   Secure the scaffolded `KnowledgeBaseArticlesController` with the `IsSuperAdmin` policy.
7.  **UI (React/Client):**
    -   A new chat widget component must be developed for the public marketing site.
    -   The widget will store a `visitorId` in `localStorage` and send it with each request.
    -   It will render the conversation based on the history returned from the backend.

### User Story: Multi-Lingual Support & Escalation Handover
**As a** Spanish-speaking user, **I want** the bot to converse with me in Spanish and, if my issue is escalated, ensure the sales team gets an accurate record.

**Technical Tasks:**
1.  **Application (Service):** In `AIChatService`:
    -   On the first message of a session (when `ChatSession` is created), call a language detection service to identify the language from `request.Text`. Store this in `ChatSession.DetectedLanguage`.
    -   When calling the AI model, include the detected language in the prompt (e.g., "You must respond in Spanish.").
2.  **Infrastructure:**
    -   Add a new `ITranslationService` interface in the Application layer.
    -   Implement `AzureTranslationService` (or similar) in the Infrastructure layer.
    -   Add a new `ICrmService` interface in the Application layer.
    -   Implement `SalesforceService` (or similar) in the Infrastructure layer.
3.  **Application (Service):** In `AIChatService`, create a method `EscalateToHumanAsync(string visitorId)`.
    -   This method is triggered by specific user input (e.g., "talk to a human") or by the lead capture flow.
    -   It retrieves the `ChatSession` and its full transcript.
    -   It calls `_translationService.TranslateAsync(transcript, "en")` to get the English version.
    -   It calls `_crmService.CreateLeadAsync()` with the original transcript, the translated transcript, and the user's contact info.
    -   It updates the `ChatSession.Status` to `Escalated`.
4.  **API:** The escalation can be triggered internally within the `RespondAsync` method, so a dedicated public API endpoint may not be necessary. An internal endpoint for `Sales Agents` to view transcripts will be needed in `ChatSessionsController`, secured with the `IsSalesAgent` policy.
5.  **UI (React/Client):** The chat widget will display the AI's lead capture message and allow the user to submit their email address.

## 4. Code Specifications (Key Logic)

### `AIChatService.cs` - Core Response & Intent Logic

```csharp
// In Diquis.Application/Services/AIChatService.cs
public async Task<PostMessageResponse> RespondAsync(PostMessageRequest request)
{
    // 1. Find or create session and transcript
    var session = await GetOrCreateSessionAsync(request.VisitorId);
    var transcript = DeserializeTranscript(session.Transcript);
    transcript.Add(new ChatMessage { Sender = "user", Text = request.Text, Timestamp = DateTime.UtcNow });

    // 2. Detect language on first message
    if (transcript.Count == 1) 
    {
        // session.DetectedLanguage = await _languageService.DetectAsync(request.Text);
    }

    // 3. Detect intent from the user's message
    string lowerCaseText = request.Text.ToLower();
    string responseText;

    if (lowerCaseText.Contains("enterprise") || lowerCaseText.Contains("federation") || lowerCaseText.Contains("api"))
    {
        // Enterprise Path
        responseText = "It sounds like our Enterprise plan is the right fit. What is the best email to reach you at to schedule a demo?";
        session.Status = ChatStatus.LeadCaptured;
    }
    else if (IsOffTopic(lowerCaseText))
    {
        // Refusal Path
        responseText = "I can only provide information about the Diquis platform. How can I help you with our features or pricing?";
    }
    else
    {
        // Knowledge Base Path
        // 4. Get knowledge base articles
        var articles = await _knowledgeRepo.ListAsync();
        var knowledge = string.Join("\n---\n", articles.Select(a => a.Content));

        // 5. Build System Prompt
        string systemPrompt = $"You are an AI sales assistant for Diquis. 
            Your responses MUST be strictly based on the following information. Do not use any other knowledge.
            Refuse to answer any off-topic questions.
            You must respond in the language with code: '{session.DetectedLanguage ?? "en"}'.

            KNOWLEDGE BASE:
            {knowledge}";
        
        // 6. Call external AI model
        // responseText = await _aiModelService.GetCompletionAsync(systemPrompt, transcript);
        responseText = "This is a placeholder AI response based on the knowledge base."; // Placeholder
    }
    
    // 7. Save conversation and return
    transcript.Add(new ChatMessage { Sender = "ai", Text = responseText, Timestamp = DateTime.UtcNow });
    session.Transcript = SerializeTranscript(transcript);
    // await _sessionRepo.UpdateAsync(session);

    return new PostMessageResponse { VisitorId = session.VisitorId, ResponseText = responseText };
}
```

### `AIChatService.cs` - Escalation Logic Pseudo-code

```csharp
// In Diquis.Application/Services/AIChatService.cs
private async Task HandleEscalationAsync(ChatSession session, string userEmail)
{
    var transcript = session.Transcript; // The original JSON transcript
    
    // 1. Machine-translate for the sales agent
    var englishTranscript = await _translationService.TranslateAsync(transcript, "en");

    // 2. Create lead in CRM
    var leadDetails = new CrmLeadInfo 
    {
        ContactEmail = userEmail,
        OriginalTranscript = transcript,
        TranslatedTranscript = englishTranscript,
        Source = "Website Chatbot"
    };
    await _crmService.CreateLeadAsync(leadDetails);

    // 3. Update session status
    session.Status = ChatStatus.Escalated;
    await _sessionRepo.UpdateAsync(session);
}
```