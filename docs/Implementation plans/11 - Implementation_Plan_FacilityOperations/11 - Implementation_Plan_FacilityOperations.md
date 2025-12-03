# Facility & Resource Operations: Implementation & Testing Plan

## 1. Executive Summary

This document outlines the implementation and testing strategy for the Facility & Resource Operations module. This system is the logistical backbone of the academy, enabling the scheduling of pitches, rooms, and equipment. Its core feature is an advanced conflict detection engine designed to maximize resource utilization by supporting complex scenarios like "pitch splitting."

The plan details the domain model for bookable resources, the backend algorithm for spatial and quantitative conflict detection, the frontend implementation of a visual resource timeline, and a testing strategy focused on validating complex, overlapping booking scenarios.

## 2. Architectural Blueprint: Domain Entities

The architecture is centered around two primary entities that model the resources themselves and the reservations placed against them. These entities will be tenant-specific.

**Action:** Create the following entity files in the `Diquis.Domain/Entities/` directory.

**File:** `Diquis.Domain/Entities/BookableResource.cs`
```csharp
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diquis.Domain.Entities;

/// <summary>
/// Represents a resource that can be booked, such as a pitch, room, or a set of equipment.
/// </summary>
public class BookableResource : BaseEntity, IMustHaveTenant
{
    public required string Name { get; set; }
    public ResourceType ResourceType { get; set; } // Enum: Pitch, Room, Equipment
    public int TotalQuantity { get; set; } = 1;
    public ResourceStatus Status { get; set; } // Enum: Available, OutOfService

    /// <summary>
    /// A flexible JSON field for resource-specific configurations.
    /// Example for a splittable pitch: {"splittable": true, "zones": ["Half A", "Half B"]}
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? Configuration { get; set; }

    public required string TenantId { get; set; }
}
```

**File:** `Diquis.Domain/Entities/Booking.cs`
```csharp
namespace Diquis.Domain.Entities;

/// <summary>
/// Represents a single reservation for a BookableResource.
/// </summary>
public class Booking : BaseEntity, IMustHaveTenant
{
    public Guid BookableResourceId { get; set; }
    public BookableResource Resource { get; set; }

    public Guid BookedById { get; set; }
    public ApplicationUser BookedBy { get; set; }

    public required string EventTitle { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    /// <summary>
    /// For splittable resources, stores which zone is booked (e.g., "Full", "Half A").
    /// </summary>
    public string? BookedZone { get; set; }

    /// <summary>
    /// For quantity-based resources, stores the number of items booked.
    /// </summary>
    public int BookedQuantity { get; set; } = 1;

    public BookingStatus Status { get; set; } // Enum: Confirmed, Pending
    public required string TenantId { get; set; }
}
```

## 3. Backend Implementation: The "Pitch Splitting" Conflict Algorithm

The core of the backend is the conflict detection logic within the `BookingService`. This algorithm must correctly handle simple time overlaps, hierarchical zone conflicts for splittable pitches, and quantity constraints for equipment.

**Action:** Implement the `CheckForConflictsAsync` method in `BookingService.cs` as the primary validation step within the booking creation process.

**File:** `Diquis.Application/Services/Bookings/BookingService.cs` (enhancement)
```csharp
private async Task CheckForConflictsAsync(Booking newBooking)
{
    var resource = await _context.BookableResources.FindAsync(newBooking.BookableResourceId);
    if (resource.Status == ResourceStatus.OutOfService)
    {
        throw new ConflictException("Resource is currently out of service.");
    }

    // 1. Get all confirmed bookings for the same resource that overlap in time.
    var conflictingTimeBookings = await _context.Bookings
        .Where(b => b.BookableResourceId == newBooking.BookableResourceId
                  && b.Status == BookingStatus.Confirmed
                  && newBooking.StartTime < b.EndTime 
                  && newBooking.EndTime > b.StartTime)
        .ToListAsync();

    if (!conflictingTimeBookings.Any()) return; // No time conflicts, no problem.

    // 2. Handle SPLITTABLE PITCH logic.
    if (resource.ResourceType == ResourceType.Pitch)
    {
        // A) If the new booking is for "Full", it conflicts with ANY existing booking.
        if (newBooking.BookedZone == "Full")
        {
            throw new ConflictException($"Cannot book full pitch; it conflicts with '{conflictingTimeBookings.First().EventTitle}'.");
        }

        // B) If ANY existing booking is for "Full", the new split booking conflicts.
        var fullPitchBooking = conflictingTimeBookings.FirstOrDefault(b => b.BookedZone == "Full");
        if (fullPitchBooking != null)
        {
            throw new ConflictException($"Cannot book a split zone; the full pitch is already booked for '{fullPitchBooking.EventTitle}'.");
        }

        // C) Check for a direct overlap on the same split zone.
        var sameZoneBooking = conflictingTimeBookings.FirstOrDefault(b => b.BookedZone == newBooking.BookedZone);
        if (sameZoneBooking != null)
        {
            throw new ConflictException($"This zone is already booked for '{sameZoneBooking.EventTitle}'.");
        }
    }
    // 3. Handle QUANTITY-BASED EQUIPMENT logic.
    else if (resource.ResourceType == ResourceType.Equipment)
    {
        var alreadyBookedQuantity = conflictingTimeBookings.Sum(b => b.BookedQuantity);
        var availableQuantity = resource.TotalQuantity - alreadyBookedQuantity;
        if (newBooking.BookedQuantity > availableQuantity)
        {
            throw new ConflictException($"Cannot book {newBooking.BookedQuantity} items; only {availableQuantity} are available.");
        }
    }
    // 4. Handle simple, non-splittable resources (e.g., a single room).
    else
    {
        throw new ConflictException($"This resource is already booked for '{conflictingTimeBookings.First().EventTitle}'.");
    }
}
```

## 4. Frontend Implementation (React)

A new feature folder will provide a visual timeline for resource scheduling.

### 4.1. Folder Structure & Setup

**Action:** Create the folder `src/features/facilities` and install the FullCalendar timeline view and its dependencies.
```bash
npm install @fullcalendar/react @fullcalendar/resource-timeline
```

### 4.2. Resource Scheduler Timeline View

The UI will use FullCalendar's timeline view to show resources vertically and their bookings horizontally over time.

**File:** `src/features/facilities/components/ResourceScheduler.tsx`
```tsx
import FullCalendar from '@fullcalendar/react';
import resourceTimelinePlugin from '@fullcalendar/resource-timeline';
import { useEffect, useState } from 'react';
import { useFacilitiesApi } from '../hooks/useFacilitiesApi'; // Custom API hook

export const ResourceScheduler = () => {
  const [resources, setResources] = useState([]);
  const [events, setEvents] = useState([]);
  const { getBookableResources, getBookings } = useFacilitiesApi();

  useEffect(() => {
    // Fetch both resources (for the vertical axis) and bookings (for the timeline events)
    Promise.all([
      getBookableResources(),
      getBookings(/* date range */)
    ]).then(([resourceData, bookingData]) => {
      
      // Format data for FullCalendar
      const formattedResources = resourceData.map(r => ({ id: r.id, title: r.name }));
      const formattedEvents = bookingData.map(b => ({
        id: b.id,
        resourceId: b.bookableResourceId,
        title: `${b.eventTitle} (${b.bookedZone || 'Full'})`,
        start: b.startTime,
        end: b.endTime,
      }));

      setResources(formattedResources);
      setEvents(formattedEvents);
    });
  }, [getBookableResources, getBookings]);

  return (
    <FullCalendar
      plugins={[resourceTimelinePlugin]}
      initialView="resourceTimelineWeek"
      schedulerLicenseKey="GPL-TO-YOUR-LICENSE-KEY" // Required for timeline view
      headerToolbar={{
        left: 'prev,next today',
        center: 'title',
        right: 'resourceTimelineDay,resourceTimelineWeek,resourceTimelineMonth'
      }}
      resourceAreaHeaderContent="Facilities"
      resources={resources}
      events={events}
    />
  );
};
```

## 5. Testing Strategy

The testing strategy must rigorously validate the complex conflict detection logic with various overlapping scenarios.

### 5.1. Backend Integration Test: Overlapping Scenarios

We will create a suite of integration tests for the `BookingService` that pre-populates a test database with bookings and then attempts to create new, conflicting bookings.

**Action:** Create a test file in `Diquis.Infrastructure.Tests`.

**File:** `Diquis.Infrastructure.Tests/Bookings/ConflictDetectionTests.cs`
```csharp
using FluentAssertions;
using Moq;
using NUnit.Framework;

[TestFixture]
public class ConflictDetectionTests
{
    private BookingService _service;
    private ApplicationDbContext _context;

    [SetUp]
    public async Task Setup()
    {
        // Use an in-memory database
        _context = /* ... get in-memory context ... */;
        _service = new BookingService(_context, /* other mocks */);

        // ARRANGE: Create a splittable pitch and a booking for "Half A" from 9 to 10 AM.
        var pitch = new BookableResource 
        { 
            Id = Guid.NewGuid(),
            Name = "Main Pitch",
            ResourceType = ResourceType.Pitch,
            Configuration = "{\"splittable\": true, \"zones\": [\"Half A\", \"Half B\"]}"
        };
        var existingBooking = new Booking
        {
            BookableResourceId = pitch.Id,
            StartTime = DateTime.Today.AddHours(9),
            EndTime = DateTime.Today.AddHours(10),
            BookedZone = "Half A",
            EventTitle = "U17 Drills",
            Status = BookingStatus.Confirmed
        };
        _context.BookableResources.Add(pitch);
        _context.Bookings.Add(existingBooking);
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task CreateBooking_WhenBookingFullPitchOverlappingASplit_ShouldThrowConflictException()
    {
        // ACT: Try to book "Full" pitch from 9:30 to 10:30 (overlaps)
        var conflictingRequest = new CreateBookingRequest 
        { 
            BookableResourceId = _context.BookableResources.First().Id,
            StartTime = DateTime.Today.AddHours(9).AddMinutes(30),
            EndTime = DateTime.Today.AddHours(10).AddMinutes(30),
            BookedZone = "Full"
        };
        
        Func<Task> act = () => _service.CreateBookingAsync(conflictingRequest);

        // ASSERT
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Cannot book full pitch; it conflicts with 'U17 Drills'.");
    }
    
    [Test]
    public async Task CreateBooking_WhenBookingDifferentSplitAtSameTime_ShouldSucceed()
    {
        // ACT: Try to book "Half B" from 9:00 to 10:00 (same time, different zone)
        var nonConflictingRequest = new CreateBookingRequest 
        { 
            BookableResourceId = _context.BookableResources.First().Id,
            StartTime = DateTime.Today.AddHours(9),
            EndTime = DateTime.Today.AddHours(10),
            BookedZone = "Half B" 
        };

        // ASSERT
        // This should complete without throwing an exception.
        await _service.CreateBookingAsync(nonConflictingRequest);
        
        var bookingsCount = await _context.Bookings.CountAsync();
        bookingsCount.Should().Be(2); // The original booking + the new one
    }
}
```
This test suite directly validates the core business rules for both conflicting and non-conflicting scenarios, ensuring the booking engine is reliable.
