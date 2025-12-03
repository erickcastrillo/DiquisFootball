# Training Sessions: Implementation & Testing Plan

## 1. Executive Summary

This document provides a comprehensive implementation and testing plan for the Training Sessions module. This module is a key functional area for coaches and directors, enabling them to schedule team practices, manage locations via integration with the facility booking system, and track player attendance.

The plan outlines the domain model, the backend scheduling logic focusing on conflict detection, the frontend implementation of a calendar view and attendance sheet, and a robust testing strategy designed to validate the critical conflict detection business rules.

## 2. Architectural Blueprint: Domain Entities

As defined in the technical guide, the module requires two primary entities, both scoped to a specific tenant to ensure data isolation.

**Action:** Create the following entity files in the `Diquis.Domain/Entities/` directory.

**File:** `Diquis.Domain/Entities/TrainingSession.cs`
```csharp
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;

namespace Diquis.Domain.Entities;

/// <summary>
/// Represents a single scheduled training practice for a team.
/// </summary>
public class TrainingSession : BaseEntity, IMustHaveTenant
{
    public Guid TeamId { get; set; }
    public Team Team { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Foreign key to the physical location (e.g., a specific field) being reserved.
    /// This links to the Facility/Resource module.
    /// </summary>
    public Guid LocationId { get; set; }
    
    /// <summary>
    /// Foreign key to the booking record created in the Facility module.
    /// Ensures that a session cannot be created without a valid location reservation.
    /// </summary>
    public Guid BookingId { get; set; }

    public string? SessionFocus { get; set; }
    public string? DrillDescription { get; set; }

    [Column(TypeName = "jsonb")]
    public string? VideoLinks { get; set; } // Serialized list of URLs

    public ICollection<TrainingAttendance> Attendances { get; set; } = new List<TrainingAttendance>();
    public required string TenantId { get; set; }
}
```

**File:** `Diquis.Domain/Entities/TrainingAttendance.cs`
```csharp
namespace Diquis.Domain.Entities;

/// <summary>
/// Records a single player's attendance status for a specific training session.
/// </summary>
public class TrainingAttendance : BaseEntity, IMustHaveTenant
{
    public Guid TrainingSessionId { get; set; }
    public TrainingSession TrainingSession { get; set; }

    public Guid PlayerId { get; set; }
    public ApplicationUser Player { get; set; }

    public AttendanceStatus AttendanceStatus { get; set; } // Enum: Pending, Present, Absent, Excused

    public DateTime MarkedAt { get; set; }
    public required string TenantId { get; set; }
}
```

## 3. Backend Implementation: Scheduling & Conflict Detection

The core of the backend logic is a transactional process for creating a session that atomically checks for internal conflicts (team/coach) and external conflicts (location availability). This requires a crucial integration with a separate `IBookingService`.

**Action:** Implement the `CreateSessionAsync` orchestration logic in `TrainingSessionService.cs`.

**File:** `Diquis.Application/Services/TrainingSessions/TrainingSessionService.cs`
```csharp
// Requires injection of ApplicationDbContext, IBookingService, ICurrentUserService
public class TrainingSessionService : ITrainingSessionService
{
    public async Task<Guid> CreateSessionAsync(CreateSessionRequest request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Step 1: Check for internal conflicts (team or coach already scheduled)
            var team = await _context.Teams.FindAsync(request.TeamId);
            if (team == null) throw new NotFoundException("Team not found.");

            var coachOrTeamConflict = await _context.TrainingSessions
                .AnyAsync(s => (s.TeamId == request.TeamId || s.Team.CoachId == team.CoachId)
                            && request.StartTime < s.EndTime && request.EndTime > s.StartTime);

            if (coachOrTeamConflict)
            {
                throw new ValidationException("The coach or team has a conflicting event at this time.");
            }

            // Step 2: Attempt to book the physical location via the Booking service.
            // This service is responsible for location-based conflict detection.
            var bookingRequest = new CreateBookingRequest
            {
                BookableResourceId = request.LocationId,
                EventTitle = $"Training: {team.Name}",
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };
            
            Guid bookingId = await _bookingService.CreateBookingAsync(bookingRequest);

            // Step 3: If booking succeeds, create and save the TrainingSession
            var newSession = _mapper.Map<TrainingSession>(request);
            newSession.BookingId = bookingId;
            // TenantId is set automatically

            _context.TrainingSessions.Add(newSession);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return newSession.Id;
        }
        catch (ConflictException ex) // Catch specific exception from BookingService
        {
            await transaction.RollbackAsync();
            throw new ValidationException($"Location conflict: {ex.Message}");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw; // Re-throw other unexpected errors
        }
    }
    // ... other methods for attendance, etc.
}
```

## 4. Frontend Implementation (React)

A new feature folder will house the UI for session scheduling and attendance tracking.

### 4.1. Folder Structure & Setup

**Action:** Create the folder `src/features/training` and install the calendar library.
```bash
npm install @fullcalendar/react @fullcalendar/daygrid @fullcalendar/timegrid @fullcalendar/interaction
```

### 4.2. Calendar View for Scheduling

**Action:** Create a `TrainingCalendar` component using FullCalendar to display sessions and handle the creation/editing of events.

**File:** `src/features/training/components/TrainingCalendar.tsx`
```tsx
import FullCalendar from '@fullcalendar/react';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { useState } from 'react';
import { SessionFormModal } from './SessionFormModal';

export const TrainingCalendar = ({ teamId }) => {
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedDateInfo, setSelectedDateInfo] = useState(null);
  // ... API hook to fetch events from `/api/trainingsessions?teamId=...`

  const handleDateClick = (arg) => {
    setSelectedDateInfo(arg);
    setModalOpen(true);
  };

  return (
    <>
      <FullCalendar
        plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
        initialView="timeGridWeek"
        events={/* events from API */}
        dateClick={handleDateClick}
        editable
        // ... other calendar options
      />
      <SessionFormModal
        show={modalOpen}
        onHide={() => setModalOpen(false)}
        dateInfo={selectedDateInfo}
        teamId={teamId}
      />
    </>
  );
};
```

### 4.3. Attendance Sheet

**Action:** Create a component to display the team roster for a given session and allow the coach to mark attendance.

**File:** `src/features/training/components/AttendanceSheet.tsx`
```tsx
import { useState, useEffect } from 'react';
import { Table, Dropdown, Button } from 'react-bootstrap';
import { useTrainingApi } from '../hooks/useTrainingApi'; // Custom API hook

export const AttendanceSheet = ({ sessionId }) => {
  const [attendance, setAttendance] = useState([]);
  const { getAttendance, markAttendance } = useTrainingApi();

  useEffect(() => {
    getAttendance(sessionId).then(setAttendance);
  }, [sessionId, getAttendance]);

  const handleStatusChange = (playerId, newStatus) => {
    setAttendance(prev => 
      prev.map(att => att.playerId === playerId ? { ...att, status: newStatus } : att)
    );
  };

  const handleSave = () => {
    markAttendance(sessionId, attendance);
  };

  return (
    <>
      <Table striped bordered hover>
        <thead><tr><th>Player Name</th><th>Status</th></tr></thead>
        <tbody>
          {attendance.map(record => (
            <tr key={record.playerId}>
              <td>{record.playerName}</td>
              <td>
                <Dropdown>
                  <Dropdown.Toggle variant="outline-secondary" size="sm">
                    {record.status}
                  </Dropdown.Toggle>
                  <Dropdown.Menu>
                    <Dropdown.Item onClick={() => handleStatusChange(record.playerId, 'Present')}>Present</Dropdown.Item>
                    <Dropdown.Item onClick={() => handleStatusChange(record.playerId, 'Absent')}>Absent</Dropdown.Item>
                    <Dropdown.Item onClick={() => handleStatusChange(record.playerId, 'Excused')}>Excused</Dropdown.Item>
                  </Dropdown.Menu>
                </Dropdown>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
      <Button onClick={handleSave}>Save Attendance</Button>
    </>
  );
};
```

## 5. Testing Strategy

The testing strategy must focus on the most complex piece of business logic: conflict detection during scheduling.

### 5.1. Backend Integration Test: Conflict Detection

This integration test will validate that the `CreateSessionAsync` method correctly identifies and prevents scheduling conflicts.

**Action:** Create a test file in `Diquis.Infrastructure.Tests`.

**File:** `Diquis.Infrastructure.Tests/Training/SchedulingConflictTests.cs`
```csharp
using FluentAssertions;
using Moq;
using NUnit.Framework;

[TestFixture]
public class SchedulingConflictTests
{
    private TrainingSessionService _service;
    private Mock<IBookingService> _mockBookingService;
    private ApplicationDbContext _context;

    [SetUp]
    public void Setup()
    {
        // Use an in-memory database
        // ...
        _mockBookingService = new Mock<IBookingService>();
        _context = /* ... get in-memory context ... */;
        _service = new TrainingSessionService(_context, _mockBookingService.Object, /* other mocks */);
    }

    [Test]
    public async Task CreateSessionAsync_WhenTeamHasOverlappingSession_ShouldThrowValidationException()
    {
        // Arrange
        // 1. Pre-load the DB with a team and a session from 9:00 to 10:30
        var teamId = Guid.NewGuid();
        _context.TrainingSessions.Add(new TrainingSession 
        { 
            TeamId = teamId, 
            StartTime = DateTime.UtcNow.Date.AddHours(9), 
            EndTime = DateTime.UtcNow.Date.AddHours(10).AddMinutes(30) 
        });
        await _context.SaveChangesAsync();

        // 2. Attempt to create a new session that overlaps (10:00 to 11:00)
        var conflictingRequest = new CreateSessionRequest 
        { 
            TeamId = teamId, 
            StartTime = DateTime.UtcNow.Date.AddHours(10), 
            EndTime = DateTime.UtcNow.Date.AddHours(11) 
        };
        
        // Act
        Func<Task> act = () => _service.CreateSessionAsync(conflictingRequest);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("The coach or team has a conflicting event at this time.");
        
        // CRITICAL: Verify the external booking service was never called
        _mockBookingService.Verify(s => s.CreateBookingAsync(It.IsAny<CreateBookingRequest>()), Times.Never);
    }

    [Test]
    public async Task CreateSessionAsync_WhenLocationIsUnavailable_ShouldThrowValidationException()
    {
        // Arrange
        // 1. No internal conflicts exist
        var request = new CreateSessionRequest 
        {
            TeamId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.Date.AddHours(10), 
            EndTime = DateTime.UtcNow.Date.AddHours(11) 
        };

        // 2. Mock the external booking service to throw a conflict exception
        _mockBookingService
            .Setup(s => s.CreateBookingAsync(It.IsAny<CreateBookingRequest>()))
            .ThrowsAsync(new ConflictException("Location is already booked at this time."));

        // Act
        Func<Task> act = () => _service.CreateSessionAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Location conflict: Location is already booked at this time.");
    }
}
```
This test suite validates both internal (team/coach) and external (location) conflict detection paths, ensuring the integrity of the scheduling system.
