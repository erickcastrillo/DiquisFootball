# Team Organization: Implementation & Testing Plan

## 1. Executive Summary

This document provides a detailed implementation and testing strategy for the Team Organization module. This module enables the core administrative function of creating teams, assigning them to divisions, and managing player rosters. It is a foundational component for structuring an academy's operations.

The plan outlines the domain entities required, the backend business logic for roster management including eligibility checks, the frontend implementation of a drag-and-drop roster builder, and a testing strategy to validate critical business rules like age restrictions and roster limits.

## 2. Architectural Blueprint: Domain Entities

Based on the technical guide, we will model the team structure with three core entities, all of which will be tenant-specific to ensure data isolation.

**Action:** Create/update the following entity files in the `Diquis.Domain/Entities/` directory.

**File:** `Diquis.Domain/Entities/Team.cs`
```csharp
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;

namespace Diquis.Domain.Entities;

/// <summary>
/// Represents a single football team within an academy.
/// </summary>
public class Team : BaseEntity, IMustHaveTenant
{
    public required string Name { get; set; }
    
    // Foreign key to the coach managing the team.
    public Guid CoachId { get; set; }
    public ApplicationUser Coach { get; set; }

    // Foreign key to the division the team competes in.
    public Guid DivisionId { get; set; }
    public Division Division { get; set; }

    public ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();
    public required string TenantId { get; set; }
}
```

**File:** `Diquis.Domain/Entities/Division.cs` (Enhancement)
```csharp
// This existing entity is extended with a Gender property.
public class Division : BaseEntity, IMustHaveTenant
{
    public required string Name { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    
    /// <summary>
    /// The gender category for this division.
    /// </summary>
    public Gender Gender { get; set; } // Enum: Male, Female, CoEd

    public required string TenantId { get; set; }
}
```

**File:** `Diquis.Domain/Entities/TeamPlayer.cs`
```csharp
namespace Diquis.Domain.Entities;

/// <summary>
/// A linking entity for the many-to-many relationship between Teams and Players (ApplicationUser).
/// </summary>
public class TeamPlayer : IMustHaveTenant
{
    public Guid TeamId { get; set; }
    public Team Team { get; set; }

    public Guid PlayerId { get; set; }
    public ApplicationUser Player { get; set; }

    public required string TenantId { get; set; }
}
```
**Note:** In `ApplicationDbContext`, the composite primary key for `TeamPlayer` would be configured using `modelBuilder.Entity<TeamPlayer>().HasKey(tp => new { tp.TeamId, tp.PlayerId });`.

## 3. Backend Implementation: Roster Management Logic

The core of this module is the business logic that governs which players are eligible for a team and the transactional nature of roster updates. This logic will be implemented in `TeamService.cs`.

### 3.1. Eligible Player Filtering

This method is crucial for providing the frontend with a list of players who can be added to a roster, enforcing age and assignment rules before the user even makes a selection.

**Action:** Implement `GetEligiblePlayersAsync` in `TeamService.cs`.

```csharp
// In Diquis.Application/Services/TeamService.cs
public async Task<List<RosterPlayerDto>> GetEligiblePlayersAsync(Guid teamId)
{
    var team = await _context.Teams
        .Include(t => t.Division)
        .FirstOrDefaultAsync(t => t.Id == teamId);
    if (team is null) throw new NotFoundException("Team not found.");

    // 1. Calculate the valid birth date range from the team's division.
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var minBirthDate = today.AddYears(-(team.Division.MaxAge + 1)).AddDays(1);
    var maxBirthDate = today.AddYears(-team.Division.MinAge);

    // 2. Get all player IDs that are already on ANY team roster for this tenant.
    var assignedPlayerIds = await _context.TeamPlayers
        .Select(tp => tp.PlayerId)
        .Distinct()
        .ToListAsync();

    // 3. Find players who are the correct gender, in the correct age range, and not already assigned.
    var eligiblePlayers = await _context.Users
        .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Player") // Is a player
                    && (team.Division.Gender == Gender.CoEd || u.Gender == team.Division.Gender) // Gender match
                    && u.DateOfBirth >= minBirthDate && u.DateOfBirth <= maxBirthDate // Age match
                    && !assignedPlayerIds.Contains(u.Id)) // Not on another team
        .ProjectTo<RosterPlayerDto>(_mapper.ConfigurationProvider)
        .ToListAsync();

    return eligiblePlayers;
}
```

### 3.2. Transactional Roster Updates

To ensure data integrity, adding and removing players from a roster will be performed within a single database transaction.

**Action:** Implement `UpdateRosterAsync` in `TeamService.cs`.

```csharp
// In Diquis.Application/Services/TeamService.cs
public async Task UpdateRosterAsync(Guid teamId, UpdateRosterRequest request)
{
    // ... Authorization check to ensure user is coach or admin ...

    await using var transaction = await _context.Database.BeginTransactionAsync();

    // 1. Validate roster size limit
    var division = await _context.Teams.Where(t => t.Id == teamId).Select(t => t.Division).SingleAsync();
    var currentCount = await _context.TeamPlayers.CountAsync(tp => tp.TeamId == teamId);
    var finalCount = currentCount - request.PlayerIdsToRemove.Count + request.PlayerIdsToAdd.Count;
    if (finalCount > division.MaxRosterSize) // Assuming MaxRosterSize is a property on Division
    {
        throw new ValidationException($"Roster exceeds the maximum size of {division.MaxRosterSize} for this division.");
    }
    
    // 2. Remove players
    await _context.TeamPlayers
        .Where(tp => tp.TeamId == teamId && request.PlayerIdsToRemove.Contains(tp.PlayerId))
        .ExecuteDeleteAsync();

    // 3. Add players
    var newRosterEntries = request.PlayerIdsToAdd.Select(pid => new TeamPlayer { TeamId = teamId, PlayerId = pid });
    await _context.TeamPlayers.AddRangeAsync(newRosterEntries);
    
    await _context.SaveChangesAsync();
    await transaction.CommitAsync();
}
```

## 4. Frontend Implementation (React)

A new feature folder will house the UI for team management, centered around a drag-and-drop interface for an intuitive user experience.

### 4.1. Folder Structure & Setup

**Action:** Create the folder `src/features/teams` and install a drag-and-drop library.
```bash
npm install @dnd-kit/core @dnd-kit/sortable
```

### 4.2. Roster Builder Component

This component will display two lists—"Eligible Players" and "Current Roster"—allowing the coach to move players between them.

**File:** `src/features/teams/components/RosterBuilder.tsx`
```tsx
import { DndContext, closestCenter } from '@dnd-kit/core';
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable';
import { useState, useEffect } from 'react';
import { PlayerColumn } from './PlayerColumn'; // A component to render one of the lists

export const RosterBuilder = ({ teamId }) => {
  const [eligiblePlayers, setEligiblePlayers] = useState([]);
  const [rosterPlayers, setRosterPlayers] = useState([]);
  // ... API hooks to fetch initial data for both lists ...

  const handleDragEnd = (event) => {
    const { active, over } = event;
    if (!over) return;

    const activeContainer = active.data.current.sortable.containerId;
    const overContainer = over.data.current.sortable.containerId;

    if (activeContainer === overContainer) return;

    // Logic to move item from one list to the other
    const itemToMove = activeContainer === 'eligible'
      ? eligiblePlayers.find(p => p.id === active.id)
      : rosterPlayers.find(p => p.id === active.id);
      
    if (overContainer === 'roster') {
      setRosterPlayers(prev => [...prev, itemToMove]);
      setEligiblePlayers(prev => prev.filter(p => p.id !== active.id));
    } else {
      setEligiblePlayers(prev => [...prev, itemToMove]);
      setRosterPlayers(prev => prev.filter(p => p.id !== active.id));
    }
  };

  const handleSaveChanges = () => {
    // Logic to calculate players added/removed and call the UpdateRosterAsync API
  };

  return (
    <DndContext collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
      <div className="row">
        <div className="col-md-6">
          <h3>Eligible Players</h3>
          <PlayerColumn id="eligible" players={eligiblePlayers} />
        </div>
        <div className="col-md-6">
          <h3>Current Roster</h3>
          <PlayerColumn id="roster" players={rosterPlayers} />
        </div>
      </div>
      <Button onClick={handleSaveChanges} className="mt-3">Save Roster</Button>
    </DndContext>
  );
};
```

## 5. Testing Strategy

The testing strategy will focus on validating the backend business rules for roster eligibility, as this is the most complex and critical part of the module.

### 5.1. Backend Integration Test: Roster Eligibility

This test ensures that the `GetEligiblePlayersAsync` method correctly filters players based on age, gender, and current team assignment.

**Action:** Create a test file in `Diquis.Infrastructure.Tests`.

**File:** `Diquis.Infrastructure.Tests/Teams/RosterEligibilityTests.cs`
```csharp
using FluentAssertions;
using NUnit.Framework;

[TestFixture]
public class RosterEligibilityTests
{
    private TeamService _service;
    private ApplicationDbContext _context;

    [SetUp]
    public async Task Setup()
    {
        // Use an in-memory database or Testcontainers
        // ...
        
        // Arrange: Pre-populate the database
        var divisionU13 = new Division { Id = Guid.NewGuid(), Name = "U13", MinAge = 12, MaxAge = 13 };
        var teamU13 = new Team { Id = Guid.NewGuid(), Name = "Lions", Division = divisionU13 };

        var playerAge12 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "player1", DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-12)) };
        var playerAge15 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "player2", DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-15)) };
        var playerAlreadyAssigned = new ApplicationUser { Id = Guid.NewGuid(), UserName = "player3", DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-12)) };

        var otherTeam = new Team { Id = Guid.NewGuid(), Name = "Tigers", Division = divisionU13 };
        
        _context.Divisions.Add(divisionU13);
        _context.Teams.AddRange(teamU13, otherTeam);
        _context.Users.AddRange(playerAge12, playerAge15, playerAlreadyAssigned);
        _context.TeamPlayers.Add(new TeamPlayer { Team = otherTeam, Player = playerAlreadyAssigned });

        await _context.SaveChangesAsync();
        _service = new TeamService(_context, /* ... mocks ... */);
    }

    [Test]
    public async Task GetEligiblePlayersAsync_ForU13Team_ShouldOnlyReturnUnassigned12YearOld()
    {
        // Act
        var teamU13 = await _context.Teams.FirstAsync(t => t.Name == "Lions");
        var eligiblePlayers = await _service.GetEligiblePlayersAsync(teamU13.Id);

        // Assert
        eligiblePlayers.Should().HaveCount(1);
        eligiblePlayers.First().UserName.Should().Be("player1");
    }
}
```
This test proves that the core filtering logic works correctly, preventing coaches from seeing ineligible players and thereby enforcing the rules at the earliest stage.
