# AI Sales Agent: Implementation & Testing Plan

## 1. Executive Summary

This document outlines the implementation and testing strategy for the AI Sales Agent module. This system will deploy an intelligent, multilingual chatbot on the public-facing marketing website to engage with visitors, answer product-related questions, and qualify leads for the human sales team.

The plan details the domain model for managing conversation history, the backend architecture for integrating with a Large Language Model (LLM) while constraining it to an approved knowledge base, the frontend implementation of a floating chat widget, and a testing strategy focused on verifying the correct context is injected into the LLM prompt.

## 2. Architectural Blueprint: The `ChatSession` Entity

To track conversations with anonymous website visitors, we will introduce a global `ChatSession` entity. This entity will exist in the `BaseDbContext` as it is not tied to a specific academy tenant.

**Action:** Create the `ChatSession.cs` entity file in the `Diquis.Domain` project.

**File:** `Diquis.Domain/Entities/ChatSession.cs`
```csharp
using Diquis.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diquis.Domain.Entities;

/// <summary>
/// Represents a single, complete conversation with an anonymous website visitor.
/// Stored in the BaseDbContext as it is not tenant-specific.
/// </summary>
public class ChatSession : BaseEntity
{
    /// <summary>
    /// A unique identifier for the anonymous visitor, typically stored in a client-side cookie.
    /// </summary>
    public required string VisitorId { get; set; }

    /// <summary>
    /// The language detected from the user's input (e.g., "en", "es").
    /// </summary>
    public string DetectedLanguage { get; set; } = "en";

    /// <summary>
    /// A JSON-serialized list of all messages in the conversation.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required string Transcript { get; set; }

    /// <summary>
    /// The current state of the conversation.
    /// </summary>
    public ChatSessionStatus Status { get; set; } = ChatSessionStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ChatSessionStatus { Active, LeadCaptured, Escalated, Abandoned }
```
**Supporting Entity:** A `KnowledgeBaseArticle` entity will also be created in the `BaseDbContext` to store the approved content that the AI is allowed to use for its answers.

## 3. Backend Implementation: LLM Integration Service

The core of the backend is the `AIChatService`, which orchestrates the conversation. Its most critical function is constructing a precise system prompt that constrains the LLM's responses to the approved knowledge base.

**Action:** Implement the `RespondAsync` method in `AIChatService.cs`, focusing on the dynamic system prompt construction.

**File:** `Diquis.Application/Services/AIChat/AIChatService.cs`
```csharp
public class AIChatService : IAIChatService
{
    private readonly IBaseRepositoryAsync<KnowledgeBaseArticle> _knowledgeRepo;
    private readonly IOpenAIService _llmService; // Injected LLM service from Infrastructure
    // ... other services

    public async Task<PostMessageResponse> RespondAsync(PostMessageRequest request)
    {
        // ... logic to get or create ChatSession and transcript ...

        // CRUCIAL: CONTEXT INJECTION
        // 1. Retrieve all enabled knowledge base articles from the database.
        var articles = await _knowledgeRepo.ListAsync(a => a.IsEnabled);
        var knowledgeBase = string.Join("\n---\n", articles.Select(a => a.Content));

        // 2. Construct the system prompt with the retrieved knowledge.
        string systemPrompt = $"""
            You are an expert AI sales assistant for Diquis, a SaaS platform for football academies.
            Your responses MUST be strictly and exclusively based on the information provided in the KNOWLEDGE BASE below.
            Do not use any external knowledge. If a question cannot be answered using the knowledge base,
            politely state that you can only answer questions about the Diquis platform.
            You must respond in the language with code: '{session.DetectedLanguage ?? "en"}'.

            KNOWLEDGE BASE:
            ---
            {knowledgeBase}
            ---
            """;
        
        // 3. Call the external LLM with the constrained prompt and conversation history.
        var responseText = await _llmService.GetCompletionAsync(systemPrompt, transcript);

        // ... logic to save transcript and return response ...
        return new PostMessageResponse { ResponseText = responseText };
    }
}
```

## 4. Frontend Implementation (React)

A new feature folder will house the floating chat widget available on the public marketing site.

### 4.1. Folder Structure

**Action:** Create the new feature folder `src/features/ai-sales`.

### 4.2. Floating Chat Widget

This component will manage the chat UI, state, and API communication.

**Action:** Create the `ChatWidget` component.

**File:** `src/features/ai-sales/components/ChatWidget.tsx`
```tsx
import { useState, useEffect, useRef } from 'react';
import { Button, Card, Form, Spinner } from 'react-bootstrap';
import { useAiSalesApi } from '../hooks/useAiSalesApi';

export const ChatWidget = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState([{ sender: 'ai', text: 'Welcome to Diquis! How can I help you today?' }]);
  const [visitorId, setVisitorId] = useState(localStorage.getItem('visitorId'));
  const { postMessage, isLoading } = useAiSalesApi();
  const messagesEndRef = useRef<null | HTMLDivElement>(null);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const handleSendMessage = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const userInput = e.currentTarget.message.value;
    if (!userInput) return;

    const newMessages = [...messages, { sender: 'user', text: userInput }];
    setMessages(newMessages);
    e.currentTarget.reset();

    const response = await postMessage({ visitorId, text: userInput });
    
    if (!visitorId) {
        const newVisitorId = response.visitorId;
        setVisitorId(newVisitorId);
        localStorage.setItem('visitorId', newVisitorId);
    }

    setMessages(prev => [...prev, { sender: 'ai', text: response.responseText }]);
  };

  if (!isOpen) {
    return <Button className="position-fixed bottom-0 end-0 m-3" onClick={() => setIsOpen(true)}>Chat with us!</Button>;
  }

  return (
    <Card className="position-fixed bottom-0 end-0 m-3" style={{ width: '350px', height: '500px' }}>
      <Card.Header>Diquis AI Assistant <Button variant="close" onClick={() => setIsOpen(false)} /></Card.Header>
      <Card.Body style={{ overflowY: 'auto' }}>
        {messages.map((msg, index) => (
          <div key={index} className={`mb-2 text-${msg.sender === 'user' ? 'end' : 'start'}`}>
            <span className={`p-2 rounded bg-${msg.sender === 'user' ? 'primary' : 'light'} text-${msg.sender === 'user' ? 'white' : 'dark'}`}>{msg.text}</span>
          </div>
        ))}
        <div ref={messagesEndRef} />
      </Card.Body>
      <Card.Footer>
        <Form onSubmit={handleSendMessage}>
          <Form.Control type="text" name="message" placeholder="Type your message..." disabled={isLoading} />
        </Form>
      </Card.Footer>
    </Card>
  );
};
```

## 5. Testing Strategy

The most critical test is to ensure that the backend is correctly injecting the approved knowledge base content into the prompt sent to the LLM. We will test the `AIChatService`'s prompt construction, not the LLM's output.

### 5.1. Backend Unit Test: Context Injection into LLM Prompt

This test will mock the database and the external LLM service to verify that the `systemPrompt` is built correctly.

**Action:** Create a unit test for the `AIChatService`.

**File:** `Diquis.Application.Tests/AIChat/ContextInjectionTests.cs`
```csharp
using FluentAssertions;
using Moq;
using NUnit.Framework;

[TestFixture]
public class ContextInjectionTests
{
    private AIChatService _service;
    private Mock<IBaseRepositoryAsync<KnowledgeBaseArticle>> _mockKnowledgeRepo;
    private Mock<IOpenAIService> _mockLlmService;

    [SetUp]
    public void Setup()
    {
        _mockKnowledgeRepo = new Mock<IBaseRepositoryAsync<KnowledgeBaseArticle>>();
        _mockLlmService = new Mock<IOpenAIService>();
        _service = new AIChatService(_mockKnowledgeRepo.Object, _mockLlmService.Object);
    }

    [Test]
    public async Task RespondAsync_ShouldInjectKnowledgeBaseContentIntoSystemPrompt()
    {
        // ARRANGE
        // 1. Define the specific knowledge we expect to be injected.
        var knowledge = "The Professional plan costs $199 per month.";
        var articles = new List<KnowledgeBaseArticle>
        {
            new() { Content = knowledge, IsEnabled = true }
        };
        _mockKnowledgeRepo.Setup(repo => repo.ListAsync(It.IsAny<Expression<Func<KnowledgeBaseArticle, bool>>>()))
                          .ReturnsAsync(articles);
        
        string capturedPrompt = null;
        // 2. Mock the LLM service to capture the prompt sent to it.
        _mockLlmService.Setup(s => s.GetCompletionAsync(It.IsAny<string>(), It.IsAny<List<ChatMessage>>()))
                       .Callback<string, List<ChatMessage>>((prompt, history) => capturedPrompt = prompt)
                       .ReturnsAsync("This is a mock response.");

        var request = new PostMessageRequest { Text = "How much is the professional plan?" };

        // ACT
        await _service.RespondAsync(request);

        // ASSERT
        // 3. Verify that the captured system prompt contains our specific knowledge base text.
        capturedPrompt.Should().NotBeNull();
        capturedPrompt.Should().Contain(knowledge, "the system prompt must include the content from the knowledge base.");
        capturedPrompt.Should().Contain("You are an expert AI sales assistant for Diquis", "the prompt must contain the correct persona definition.");
    }
}
```
This test provides high confidence that the core mechanism for constraining the LLM is working correctly, ensuring the AI agent provides accurate, approved information.
