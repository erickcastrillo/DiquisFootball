# AI Governance & Safety: Implementation & Testing Plan

## 1. Executive Summary

This document outlines the implementation and testing strategy for the AI Governance & Safety Protocols (Module 16-D). Unlike a typical feature, this module establishes a set of mandatory, cross-cutting architectural patterns and development practices to ensure all AI-powered features are secure, auditable, and transparent.

The core of this framework is the "Human-in-the-Loop" (HITL) mandate, which requires explicit user approval for all high-consequence AI actions. This plan details the domain model for logging these interactions, the reusable frontend components for displaying liability disclaimers, and a testing strategy to enforce the HITL workflow.

## 2. Architectural Blueprint: The Audit Trail Entity

To create an immutable audit trail for all high-consequence AI actions, a new logging entity is required. This entity is the cornerstone of the "propose and dispose" workflow, capturing every step of the decision-making process.

**Action:** Create the `AIInteractionLog` entity in the `Diquis.Domain` project.

**File:** `Diquis.Domain/Entities/AIInteractionLog.cs`
```csharp
namespace Diquis.Domain.Entities;

/// <summary>
/// Creates an immutable audit trail for high-consequence AI actions,
/// capturing the "propose and dispose" workflow.
/// </summary>
public class AIInteractionLog : BaseEntity, IMustHaveTenant
{
    public required string AIModule { get; set; } // e.g., "AutomatedMatchReport"
    public ProposedActionType ProposedActionType { get; set; }

    /// <summary>
    /// A JSON snapshot of the AI-generated content or proposed action.
    /// </summary>
    public required string ProposedData { get; set; }

    public AIInteractionStatus Status { get; set; } = AIInteractionStatus.Proposed;
    
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }

    public required string TenantId { get; set; }
}

public enum ProposedActionType { ExternalCommunication, FinancialTransaction, DestructiveOperation }
public enum AIInteractionStatus { Proposed, Approved, Rejected }
```

## 3. Backend Implementation: The "Propose & Dispose" Pattern

The HITL mandate will be enforced through a strict, two-step service pattern for all high-consequence actions. No service will execute an action directly. Instead, it will propose the action and wait for a separate, explicit approval.

**Action:** Implement all high-consequence AI features using the following two-step pattern.

**File:** `Diquis.Application/Services/AI/AICoachingService.cs` (Illustrative Pattern)
```csharp
// Illustrating the pattern for the Automated Match Report feature.

// Step 1: The "Propose" Method
// This method generates content but does NOT send it. It creates a log entry.
public async Task<Guid> GenerateFeedbackAsync(GenerateFeedbackRequest request)
{
    // 1. Generate the list of personalized messages
    var generatedMessages = await _aiModel.GenerateAllMessagesAsync(request);
    
    // 2. Create the auditable log entry with the proposed data
    var logEntry = new AIInteractionLog
    {
        AIModule = "AutomatedMatchReport",
        ProposedActionType = ProposedActionType.ExternalCommunication,
        ProposedData = System.Text.Json.JsonSerializer.Serialize(generatedMessages),
        Status = AIInteractionStatus.Proposed,
        // TenantId is set automatically by the DbContext
    };

    await _context.AIInteractionLogs.AddAsync(logEntry);
    await _context.SaveChangesAsync();
    
    // 3. Return the ID of the log entry, which acts as a token for the approval step.
    return logEntry.Id;
}

// Step 2: The "Dispose" (Approve) Method
// This method is called by a separate, secure API endpoint after user review.
public async Task ApproveAndSendFeedbackAsync(Guid interactionLogId)
{
    var logEntry = await _context.AIInteractionLogs.FindAsync(interactionLogId);

    // 1. Validate the log entry and user permissions
    if (logEntry is null || logEntry.Status != AIInteractionStatus.Proposed)
        throw new InvalidOperationException("This action is not pending approval.");
    if (!_currentUser.HasPermission("permission:coaching.ai.approve"))
        throw new ForbiddenAccessException("User cannot approve this action.");

    // 2. Update the log to mark the action as approved
    logEntry.Status = AIInteractionStatus.Approved;
    logEntry.ReviewedByUserId = _currentUserService.UserId;
    logEntry.ReviewedAtUtc = DateTime.UtcNow;

    // 3. ONLY NOW, enqueue the actual work to be done by a background job
    var messages = System.Text.Json.JsonSerializer.Deserialize<List<FeedbackMessage>>(logEntry.ProposedData);
    foreach (var message in messages)
    {
        _jobService.Enqueue<ICommunicationService>(s => s.SendEmailAsync(message.Recipient, message.Content));
    }
    
    await _context.SaveChangesAsync();
}
```

## 4. Frontend Implementation (React)

To ensure users are aware of their responsibilities when consuming AI-generated content, a standardized, reusable disclaimer component will be implemented and mandated across the application.

### 4.1. Reusable Disclaimer Component

**Action:** Create a new reusable `AIDisclaimer` component.

**File:** `src/components/ui/AIDisclaimer.tsx`
```tsx
import React from 'react';

const disclaimerText = {
  coaching: "This content is AI-generated. The Certified Professional (Coach) remains fully responsible for verifying its accuracy and ensuring the physical safety of all participants before and during use.",
  financial: "This content is an AI-generated prediction based on historical data and is not a guarantee of future outcomes. The Certified Professional (Owner/Accountant) remains fully responsible for all financial and operational decisions.",
  medical: "This content is AI-generated and for informational purposes only. The Certified Professional (Doctor/Physio) remains fully responsible for any diagnosis or treatment decisions."
};

type DisclaimerType = keyof typeof disclaimerText;

interface AIDisclaimerProps {
  type: DisclaimerType;
  className?: string;
}

export const AIDisclaimer: React.FC<AIDisclaimerProps> = ({ type, className = '' }) => {
  return (
    <div className={`text-muted fst-italic small p-2 mt-3 bg-light ${className}`}>
      <p className="mb-0">{disclaimerText[type]}</p>
    </div>
  );
};
```
**Implementation Mandate:** This component must be added to all pages displaying advisory AI content, such as the Training Session Generator (Module 16-B) and the Cash Flow Forecast page (Module 16-C).

## 5. Testing Strategy

The testing strategy must rigorously verify that the "Human-in-the-Loop" workflow is correctly implemented and that no high-consequence action can be performed without being logged first.

### 5.1. Backend Test: Verifying the HITL Audit Trail

This unit test ensures that the "propose" step of any high-consequence action correctly creates a log entry and does **not** perform the final action.

**Action:** Create a unit test for the `GenerateFeedbackAsync` method in the `AICoachingService`.

**File:** `Diquis.Application.Tests/Services/AICoachingServiceTests.cs` (Illustrative Test)
```csharp
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

[TestFixture]
public class AICoachingServiceTests
{
    private Mock<DbSet<AIInteractionLog>> _mockLogs;
    private Mock<IBackgroundJobService> _mockJobService;
    private AICoachingService _service;

    [SetUp]
    public void Setup()
    {
        _mockLogs = new Mock<DbSet<AIInteractionLog>>();
        _mockJobService = new Mock<IBackgroundJobService>();
        // Mock DbContext to use the mock DbSet
        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(c => c.AIInteractionLogs).Returns(_mockLogs.Object);
        
        _service = new AICoachingService(mockContext.Object, _mockJobService.Object /*, other mocks */);
    }

    [Test]
    public async Task GenerateFeedbackAsync_ShouldCreateLogEntryAndNotSendEmail()
    {
        // Arrange
        var request = new GenerateFeedbackRequest();
        // Mock AI model to return some data
        _mockAiModel.Setup(m => m.GenerateAllMessagesAsync(request)).ReturnsAsync(new List<FeedbackMessage> { /* ... */ });

        // Act
        var interactionLogId = await _service.GenerateFeedbackAsync(request);

        // Assert
        // 1. Verify that an AIInteractionLog WAS created.
        _mockLogs.Verify(
            logs => logs.AddAsync(It.Is<AIInteractionLog>(log => 
                log.Status == AIInteractionStatus.Proposed &&
                log.ProposedActionType == ProposedActionType.ExternalCommunication
            ), It.IsAny<CancellationToken>()), 
            Times.Once(),
            "The service must create a 'Proposed' log entry in the database."
        );

        // 2. CRUCIAL: Verify that the final action (sending an email) was NOT performed.
        _mockJobService.Verify(
            job => job.Enqueue<ICommunicationService>(It.IsAny<Expression<Action<ICommunicationService>>>()),
            Times.Never(),
            "The 'propose' step must NEVER enqueue the final action. It must only log the proposal."
        );

        // 3. Verify the context was saved.
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
}
```
This test provides high confidence that the system adheres to the "propose and dispose" pattern, ensuring no high-consequence action can be taken without first creating an auditable log entry that awaits explicit user approval.
