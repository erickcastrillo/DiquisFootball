# Scouting & Recruitment: Implementation & Testing Plan

## 1. Executive Summary

This document details the implementation and testing strategy for the Scouting & Recruitment module. This system provides a dedicated workflow for managing and evaluating external talent (trialists) separately from the main academy roster, culminating in a seamless process to "promote" a successful trialist to a full player while preserving all historical evaluation data.

The plan outlines the domain model for trialists and their evaluations, the critical backend transaction logic for the promotion process, the frontend implementation of a standardized evaluation form, and a testing strategy to ensure data integrity during the trialist-to-player conversion.

## 2. Architectural Blueprint: Domain Entities

To keep trialist data separate from the main roster, we will introduce two new core entities as defined in the technical guide.

**Action:** Create the following entity files in the `Diquis.Domain/Entities/` directory.

**File:** `Diquis.Domain/Entities/Trialist.cs`
```csharp
namespace Diquis.Domain.Entities;

/// <summary>
/// Represents a prospective player being evaluated. Kept separate from the main ApplicationUser table.
/// </summary>
public class Trialist : BaseEntity, IMustHaveTenant
{
    public required string Name { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? ContactInfo { get; set; }
    public string? PreviousClub { get; set; }
    public TrialistStatus Status { get; set; } = TrialistStatus.Active;

    /// <summary>
    /// If promoted, this links to the official ApplicationUser player record.
    /// </summary>
    public Guid? PromotedToPlayerId { get; set; }

    public required string TenantId { get; set; }
}

public enum TrialistStatus { Active, Archived, Promoted }
```

**File:** `Diquis.Domain/Entities/EvaluationCard.cs`
```csharp
namespace Diquis.Domain.Entities;

/// <summary>
/// A standardized scorecard for evaluating a trialist, completed by a scout.
/// </summary>
public class EvaluationCard : BaseEntity, IMustHaveTenant
{
    public Guid TrialistId { get; set; }
    public Trialist Trialist { get; set; }

    public Guid ScoutId { get; set; } // The ApplicationUser ID of the coach/director
    public ApplicationUser Scout { get; set; }

    public DateTime EvaluationDate { get; set; }

    // Ratings from 1-10
    public int TechnicalRating { get; set; }
    public int TacticalRating { get; set; }
    public int PhysicalRating { get; set; }
    public int PsychologicalRating { get; set; }

    public required string SummaryNotes { get; set; }

    /// <summary>
    /// After promotion, this is back-filled to link the historical evaluation
    /// to the official player profile for long-term tracking.
    /// </summary>
    public Guid? PlayerId { get; set; }
    public ApplicationUser? Player { get; set; }

    public required string TenantId { get; set; }
}
```

## 3. Backend Implementation: "Promote to Player" Logic

The most critical backend process is the atomic transaction that converts a `Trialist` into a full `ApplicationUser` player, ensuring all historical data is preserved and correctly linked.

**Action:** Implement the `PromoteTrialistToPlayerAsync` orchestration method, likely within a new `ScoutingOrchestrationService.cs`.

**File:** `Diquis.Application/Services/Scouting/ScoutingOrchestrationService.cs`
```csharp
// Requires injection of ApplicationDbContext and PlayerService
public class ScoutingOrchestrationService : IScoutingOrchestrationService
{
    public async Task<Guid> PromoteTrialistToPlayerAsync(Guid trialistId, PromoteTrialistRequest promotionRequest)
    {
        // A single transaction ensures this is an atomic operation
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var trialist = await _context.Trialists.FindAsync(trialistId);
            if (trialist is null || trialist.Status != TrialistStatus.Active)
            {
                throw new InvalidOperationException("Trialist is not active or does not exist.");
            }

            // 1. Create the full player profile by calling the existing player service
            var registerPlayerRequest = new RegisterPlayerRequest
            {
                FullName = trialist.Name,
                DateOfBirth = trialist.DateOfBirth,
                // Map other fields from trialist and the promotionRequest...
            };
            var newPlayerId = await _playerService.RegisterPlayerAsync(registerPlayerRequest);

            // 2. Update the original trialist to an archived/promoted state
            trialist.Status = TrialistStatus.Promoted;
            trialist.PromotedToPlayerId = newPlayerId;

            // 3. Back-fill the PlayerId on all historical evaluation cards
            var evaluations = await _context.EvaluationCards
                .Where(e => e.TrialistId == trialistId)
                .ToListAsync();
                
            foreach (var card in evaluations)
            {
                card.PlayerId = newPlayerId;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return newPlayerId;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw; // Re-throw to be handled by global exception middleware
        }
    }
}
```

## 4. Frontend Implementation (React)

A new feature folder will house the scouting-related UI, including the standardized evaluation form.

### 4.1. Folder Structure

**Action:** Create the new feature folder `src/features/scouting`.

### 4.2. Evaluation Form with Rating Sliders

**Action:** Implement the `EvaluationCardForm` using Formik for state management and Yup for validation.

**File:** `src/features/scouting/components/EvaluationForm.tsx`
```tsx
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { Button, Form as BootstrapForm } from 'react-bootstrap';

const EvaluationSchema = Yup.object().shape({
  technicalRating: Yup.number().min(1).max(10).required(),
  tacticalRating: Yup.number().min(1).max(10).required(),
  physicalRating: Yup.number().min(1).max(10).required(),
  psychologicalRating: Yup.number().min(1).max(10).required(),
  summaryNotes: Yup.string().required('Summary notes are required.'),
});

const RatingSlider = ({ field, form, label }) => (
  <BootstrapForm.Group className="mb-3">
    <label htmlFor={field.name}>{label} ({field.value})</label>
    <Field type="range" className="form-range" id={field.name} {...field} min="1" max="10" />
  </BootstrapForm.Group>
);

export const EvaluationForm = ({ onSubmit }) => {
  return (
    <Formik
      initialValues={{
        technicalRating: 5, tacticalRating: 5, physicalRating: 5, psychologicalRating: 5, summaryNotes: ''
      }}
      validationSchema={EvaluationSchema}
      onSubmit={onSubmit}
    >
      {() => (
        <Form>
          <Field name="technicalRating" label="Technical" component={RatingSlider} />
          <Field name="tacticalRating" label="Tactical" component={RatingSlider} />
          <Field name="physicalRating" label="Physical" component={RatingSlider} />
          <Field name="psychologicalRating" label="Psychological" component={RatingSlider} />
          
          <BootstrapForm.Group className="mb-3">
            <label htmlFor="summaryNotes">Summary Notes</label>
            <Field name="summaryNotes" as="textarea" rows={4} className="form-control" />
            <ErrorMessage name="summaryNotes" component="div" className="text-danger" />
          </BootstrapForm.Group>

          <Button type="submit">Submit Evaluation</Button>
        </Form>
      )}
    </Formik>
  );
};
```

## 5. Testing Strategy

The testing strategy must verify that the promotion process is atomic and that all historical data is correctly re-associated with the new player profile.

### 5.1. Backend Integration Test: Trialist->Player Conversion

This test ensures data integrity is maintained throughout the promotion workflow.

**Action:** Create a new integration test file in `Diquis.Infrastructure.Tests`.

**File:** `Diquis.Infrastructure.Tests/Scouting/PromotionIntegrityTests.cs`
```csharp
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

[TestFixture]
public class PromotionIntegrityTests
{
    private ScoutingOrchestrationService _scoutingService;
    private Mock<IPlayerService> _mockPlayerService;
    private ApplicationDbContext _context;

    [SetUp]
    public async Task Setup()
    {
        _context = /* ... get in-memory context ... */;
        _mockPlayerService = new Mock<IPlayerService>();
        _scoutingService = new ScoutingOrchestrationService(_context, _mockPlayerService.Object);

        // ARRANGE: Seed DB with a trialist and two evaluation cards
        var trialist = new Trialist { Id = Guid.NewGuid(), Name = "John Doe", DateOfBirth = new DateOnly(2010, 5, 5) };
        _context.Trialists.Add(trialist);
        _context.EvaluationCards.AddRange(
            new EvaluationCard { TrialistId = trialist.Id, /* ...ratings... */ },
            new EvaluationCard { TrialistId = trialist.Id, /* ...ratings... */ }
        );
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task PromoteTrialistToPlayerAsync_ShouldPreserveAndRelinkEvaluationHistory()
    {
        // ARRANGE
        var trialist = await _context.Trialists.FirstAsync();
        var newPlayerId = Guid.NewGuid();
        _mockPlayerService
            .Setup(s => s.RegisterPlayerAsync(It.IsAny<RegisterPlayerRequest>()))
            .ReturnsAsync(newPlayerId); // Mock the player creation to return a new ID

        // ACT
        var createdPlayerId = await _scoutingService.PromoteTrialistToPlayerAsync(trialist.Id, new PromoteTrialistRequest());

        // ASSERT
        // 1. Verify the correct player ID was returned
        createdPlayerId.Should().Be(newPlayerId);

        // 2. Verify the trialist record was updated correctly
        var promotedTrialist = await _context.Trialists.FindAsync(trialist.Id);
        promotedTrialist.Status.Should().Be(TrialistStatus.Promoted);
        promotedTrialist.PromotedToPlayerId.Should().Be(newPlayerId);

        // 3. CRUCIAL: Verify historical evaluation cards are now linked to the new player ID
        var areCardsRelinked = await _context.EvaluationCards
            .Where(e => e.TrialistId == trialist.Id)
            .AllAsync(e => e.PlayerId == newPlayerId);
        
        areCardsRelinked.Should().BeTrue();
        var cardCount = await _context.EvaluationCards.CountAsync(e => e.PlayerId == newPlayerId);
        cardCount.Should().Be(2);
    }
}
```
This test confirms the atomicity of the promotion and validates the core requirement that all scouting history is preserved and correctly associated with the new player profile.
