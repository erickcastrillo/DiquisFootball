# Technical Implementation Guide: Facility & Resource Operations

This document provides a detailed technical guide for implementing the "Facility & Resource Operations" module. The core of this system is a robust booking engine with advanced conflict detection for spatially divisible and quantity-based resources.

## 1. Architectural Analysis

### Domain Entities

To model the booking system, the following entities are required:

1.  **`BookableResource`**: The central entity representing anything that can be reserved.
    *   `Name` (string, required, e.g., "Main Pitch," "GPS Vests").
    *   `ResourceType` (enum: `Pitch`, `Room`, `Equipment`).
    *   `TotalQuantity` (int, default: 1): For quantity-based resources like "GPS Vests."
    *   `Status` (enum: `Available`, `OutOfService`).
    *   `Configuration` (JSON, string): A flexible field to store layout details. For a pitch, this could be `{"splittable": true, "zones": ["Half A", "Half B", "Quarter A", ...]}`.

2.  **`Booking`**: Represents a single reservation event.
    *   `BookableResourceId` (Guid, FK to `BookableResource`).
    *   `BookedById` (Guid, FK to `ApplicationUser`).
    *   `EventTitle` (string).
    *   `StartTime` (DateTime, required).
    *   `EndTime` (DateTime, required).
    *   `BookedZone` (string, nullable): For splittable resources, stores which part is booked (e.g., "Full", "Half A").
    *   `BookedQuantity` (int, default: 1): For quantity-based resources.
    *   `Status` (enum: `Confirmed`, `Pending`): To support the booking request workflow for coaches.

### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `BookableResource`: Facilities are exclusive to an academy.
    -   `Booking`: Bookings are exclusive to an academy.

### Permissions & Authorization

| FRS Role | Policy Name | Permissions Claim | Booking Status on Creation |
| :--- | :--- | :--- | :--- |
| `academy_admin` | `IsAcademyAdmin`| `permission:facility.admin` | `Confirmed` |
| `director_of_football`| `IsDirector` | `permission:facility.book` | `Confirmed` |
| `coach` | `IsCoach` | `permission:facility.request` | `Pending` |

## 2. Scaffolding Strategy (CLI)

Execute these commands from the `Diquis.Application/Services` directory.

1.  **Bookable Resource Management (for Admins):**
    ```bash
    dotnet new nano-service -s BookableResource -p BookableResources -ap Diquis
    dotnet new nano-controller -s BookableResource -p BookableResources -ap Diquis
    ```

2.  **Booking Management (for all booking actors):**
    ```bash
    dotnet new nano-service -s Booking -p Bookings -ap Diquis
    dotnet new nano-controller -s Booking -p Bookings -ap Diquis
    ```

## 3. Implementation Plan (Agile Breakdown)

### User Story: Pitch Booking and Splitting
**As a** `director_of_football`, **I want** a visual calendar to book a full pitch or split it into zones, **so that** I can schedule multiple groups at once.

**Technical Tasks:**
1.  **Domain:** Create the `BookableResource` and `Booking` entities as defined above.
2.  **Persistence:** Add `DbSet<BookableResource>` and `DbSet<Booking>` to `ApplicationDbContext`.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddFacilityBookingEntities`
3.  **Application (DTOs):**
    -   `BookingDto`: Will represent a booking event for the calendar view.
    -   `CreateBookingRequest`: Will contain all necessary fields to create a new booking.
4.  **Application (Service):** The `BookingService.cs` is the core of this module. The `CreateBookingAsync` method will contain all conflict detection logic.
    -   It will first check the user's role. If the user is a `coach`, the new `Booking` entity's `Status` will be set to `Pending`. Otherwise, it will be `Confirmed`.
    -   **Crucial:** Before saving, it will call a private `CheckForConflictsAsync(newBooking)` method.
5.  **API:** In `BookingsController.cs`:
    -   `GET /api/bookings?start={date}&end={date}`: Fetches all confirmed bookings within a date range to populate the calendar.
    -   `POST /api/bookings`: Accepts a `CreateBookingRequest` and calls the `CreateBookingAsync` service method. This endpoint will be secured to allow access by all booking roles.
    -   `PUT /api/bookings/{id}/confirm`: An admin-only endpoint to approve a pending booking.
6.  **UI (React/Client):**
    -   Create a `src/pages/facilities/CalendarView.tsx` page.
    -   **Crucial:** Integrate a full-featured calendar library like `fullcalendar.io` with its React adapter.
    -   The calendar will fetch events from the `GET /api/bookings` endpoint.
    -   Clicking on the calendar will open a `<BookingForm />` modal, which will allow users to select the resource, time, and zone/quantity.

### User Story: Real-Time Conflict Detection
**As the** System, **I must** instantly detect and report booking conflicts to prevent double-booking.

**Technical Tasks:**
1.  **Application (Service):** This is not a separate feature but the core logic within `BookingService.CreateBookingAsync`.
    -   Implement the `CheckForConflictsAsync(Booking newBooking)` method.
    -   This method must handle two distinct types of conflict: **Spatial** (for pitches) and **Quantitative** (for equipment).
    -   If a conflict is detected, the method must throw a custom `ConflictException` containing a user-friendly error message (e.g., "Conflicts with 'U17 Training' on Pitch 1 - Half A").
    -   The global exception handler middleware will catch this `ConflictException` and return a `409 Conflict` HTTP status code to the client.
2.  **UI (React/Client):** The client-side form submission logic must be prepared to handle a `409` response and display the error message from the response body to the user.

## 4. Code Specifications (Key Logic)

### `BookingService.cs` - Create and Conflict Check Orchestration

```csharp
// Inside BookingService.cs

public async Task<Guid> CreateBookingAsync(CreateBookingRequest request)
{
    var newBooking = _mapper.Map<Booking>(request);
    
    // Set status based on role (pseudo-code)
    var userRole = _currentUserService.Role; // Assuming service gives user role
    newBooking.Status = (userRole == "Coach") ? BookingStatus.Pending : BookingStatus.Confirmed;

    // Do not check for conflicts if the booking is just a pending request
    if (newBooking.Status == BookingStatus.Confirmed)
    {
        await CheckForConflictsAsync(newBooking);
    }
    
    _context.Bookings.Add(newBooking);
    await _context.SaveChangesAsync();
    
    // If pending, enqueue a notification for admins
    if (newBooking.Status == BookingStatus.Pending)
    {
        // _jobService.Enqueue<INotificationService>(s => s.NotifyAdminsOfBookingRequest(newBooking.Id));
    }

    return newBooking.Id;
}
```

### `BookingService.cs` - The Conflict Detection Logic

```csharp
private async Task CheckForConflictsAsync(Booking newBooking)
{
    var resource = await _context.BookableResources.FindAsync(newBooking.BookableResourceId);
    if (resource.Status == ResourceStatus.OutOfService)
    {
        throw new ConflictException("Resource is currently out of service.");
    }

    // Get all confirmed bookings for the same resource that overlap in time
    var conflictingTimeBookings = await _context.Bookings
        .Where(b => b.BookableResourceId == newBooking.BookableResourceId
                  && b.Status == BookingStatus.Confirmed
                  && newBooking.StartTime < b.EndTime 
                  && newBooking.EndTime > b.StartTime)
        .ToListAsync();

    if (!conflictingTimeBookings.Any()) return; // No time conflicts, no problem

    // --- Logic for Splittable Pitches ---
    if (resource.ResourceType == ResourceType.Pitch)
    {
        // Case 1: New booking is for the full pitch
        if (newBooking.BookedZone == "Full" && conflictingTimeBookings.Any())
        {
            throw new ConflictException($"Cannot book full pitch; it conflicts with {conflictingTimeBookings.Count} other booking(s).");
        }

        // Case 2: Any existing booking is for the full pitch
        if (conflictingTimeBookings.Any(b => b.BookedZone == "Full"))
        {
            throw new ConflictException("Cannot book a split zone; the full pitch is already booked.");
        }

        // Case 3: Check for direct zone overlap
        var zoneConflict = conflictingTimeBookings.FirstOrDefault(b => b.BookedZone == newBooking.BookedZone);
        if (zoneConflict != null)
        {
            throw new ConflictException($"This zone is already booked for '{zoneConflict.EventTitle}'.");
        }
    }
    // --- Logic for Quantity-Based Equipment ---
    else if (resource.ResourceType == ResourceType.Equipment)
    {
        var bookedQuantity = conflictingTimeBookings.Sum(b => b.BookedQuantity);
        var availableQuantity = resource.TotalQuantity - bookedQuantity;

        if (newBooking.BookedQuantity > availableQuantity)
        {
            throw new ConflictException($"Cannot book {newBooking.BookedQuantity} items; only {availableQuantity} are available at this time.");
        }
    }
    // --- Logic for simple, non-splittable resources (e.g., Room) ---
    else
    {
        if (conflictingTimeBookings.Any())
        {
            throw new ConflictException($"This resource is already booked for '{conflictingTimeBookings.First().EventTitle}'.");
        }
    }
}
```