# Technical Implementation Guide: Training Sessions (Module 4)

This document provides a detailed technical guide for implementing the "Training Sessions" module as specified in the corresponding Functional Requirement Specification (FRS).

## 1. Architectural Analysis

This module introduces the core functionality for scheduling practices and tracking player attendance. It has a critical dependency on the Facility & Resource Operations module for location booking and conflict detection.

### Domain Entities

1.  **`TrainingSession`**: The central entity representing a single scheduled practice.
    *   `TeamId` (Guid, required, FK to `Team`).
    *   `StartTime` (DateTime, required).
    *   `EndTime` (DateTime, required).
    *   `LocationId` (Guid, required, FK to `BookableResource`).
    *   `SessionFocus` (string, optional).
    *   `DrillDescription` (string, optional, Markdown/Text format).
    *   `VideoLinks` (JSON, string): A serialized list of URL strings.
    *   `BookingId` (Guid, required, FK to `Booking`): A direct link to the booking record created in the Facility module to reserve the location.

2.  **`TrainingAttendance`**: A linking entity to record the attendance status of each player for a session.
    *   `TrainingSessionId` (Guid, required, FK to `TrainingSession`).
    *   `PlayerId` (Guid, required, FK to `ApplicationUser`).
    *   `AttendanceStatus` (enum: `Pending`, `Present`, `Absent`, `Excused`).
    *   `MarkedAt` (DateTime).

### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `TrainingSession`: Training sessions are exclusive to an academy.
    -   `TrainingAttendance`: Attendance records are exclusive to an academy.

### Permissions & Authorization

| FRS Role | Policy Name | Permissions Claim |
| :--- | :--- | :--- |
| `director_of_football`| `IsDirector` | `permission:sessions.manage.all` |
| `coach` | `IsCoach` | `permission:sessions.manage.ownteam` |
| `player` / `parent` | `IsEndUser` | `permission:sessions.read` |

**Note:** The `IsCoach` policy will require runtime validation to ensure a coach can only manage sessions for their assigned teams.

## 2. Scaffolding Strategy (CLI)

Execute these commands from the `Diquis.Application/Services` directory.

```bash
# Service for managing Training Sessions and their related data
dotnet new nano-service -s TrainingSession -p TrainingSessions -ap Diquis
dotnet new nano-controller -s TrainingSession -p TrainingSessions -ap Diquis

# Attendance is not a separate CRUD entity; it will be managed via the TrainingSessionService.
```

## 3. Implementation Plan (Agile Breakdown)

### User Story: Session Scheduling
**As a** `coach`, **I need to** schedule new training sessions, ensuring there are no location or team conflicts.

**Technical Tasks:**
1.  **Domain:** Create the `TrainingSession` and `TrainingAttendance` entities in `Diquis.Domain`.
2.  **Persistence:** Add `DbSet<TrainingSession>` and `DbSet<TrainingAttendance>` to `ApplicationDbContext`. Create a migration.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddTrainingSessionEntities`
3.  **Application (DTOs):** Create `TrainingSessionDto`, `CreateSessionRequest`, and `UpdateSessionRequest`.
4.  **Application (Service):** In `TrainingSessionService.cs`, implement `CreateSessionAsync`.
    -   **Crucial Integration:** This service **must** inject `IBookingService` from the Facility module.
    -   **Crucial Validation:** The `CreateSessionAsync` method will be a transactional operation that:
        1.  Checks for team/coach schedule conflicts by querying existing `TrainingSession` records.
        2.  Calls `_bookingService.CreateBookingAsync()` to reserve the physical location. The booking service will automatically handle location conflict detection.
        3.  If the booking is successful, it creates the `TrainingSession` entity, storing the returned `BookingId`.
        4.  If any step fails, the entire transaction is rolled back.
5.  **API:** In `TrainingSessionsController.cs`:
    -   `POST /`: Calls `CreateSessionAsync`. Secured for `IsDirector` and `IsCoach`.
    -   `PUT /{id}`: Calls `UpdateSessionAsync`.
    -   `GET /?teamId={id}&start={date}&end={date}`: Fetches sessions for a team calendar view.
6.  **UI (React/Client):**
    -   Create `src/pages/schedule/TeamCalendarPage.tsx`.
    -   Integrate a calendar library (e.g., `fullcalendar.io`).
    -   Clicking the calendar opens a `<SessionForm />` modal to create/edit sessions. The Location dropdown will be populated from the `BookableResources` API.

### User Story: Attendance Marking
**As a** `coach`, **I need to** take attendance for each training session to track player participation.

**Technical Tasks:**
1.  **Application (DTOs):** Create `PlayerAttendanceDto` (`PlayerId`, `PlayerName`, `AttendanceStatus`) and `MarkAttendanceRequest` (`List<PlayerAttendanceDto>`).
2.  **Application (Service):** In `TrainingSessionService.cs`:
    -   Implement `GetAttendanceForSessionAsync(Guid sessionId)`:
        -   Fetches the session and its team roster. For each player on the roster, it finds their corresponding `TrainingAttendance` record or defaults to `Pending`.
    -   Implement `MarkAttendanceAsync(Guid sessionId, MarkAttendanceRequest request)`:
        -   **Crucial Authorization:** Verify the current user is the coach for this session's team or a director.
        -   **Crucial Validation:** Throw an exception if `session.StartTime` is in the future.
        -   For each DTO in the request, it will `UPSERT` a `TrainingAttendance` record.
3.  **API:** In `TrainingSessionsController.cs`:
    -   `GET /{id}/attendance`: Calls `GetAttendanceForSessionAsync`.
    -   `PUT /{id}/attendance`: Calls `MarkAttendanceAsync`.
4.  **UI (React/Client):**
    -   Create `src/pages/sessions/{id}/AttendancePage.tsx`.
    -   Display a list of players from the team roster.
    -   Each player row has a dropdown (`Present`, `Absent`, `Excused`). A "Save" button submits the list to the `PUT /{id}/attendance` endpoint.
    -   The page should show a message like "Attendance can be marked after the session has started" if the session is in the future.

### User Story: Session Content & Video Links
**As a** `director_of_football`, **I want to** attach training plans and video links to a session.

**Technical Tasks:**
1.  **Domain:** The `TrainingSession` entity already includes `DrillDescription` and `VideoLinks` (as a JSON string).
2.  **Application (Service):** The `CreateSessionAsync` and `UpdateSessionAsync` methods in `TrainingSessionService.cs` will handle mapping these fields from their request DTOs.
    -   **Crucial Validation:** Add a check to ensure any string in the `VideoLinks` list is a valid URL format.
3.  **API:** The existing `POST /` and `PUT /{id}` endpoints in `TrainingSessionsController.cs` will be updated to accept the new content fields in their request bodies.
4.  **UI (React/Client):**
    -   The `<SessionForm />` modal will be updated with a text area for "Drill Description" and a dynamic list component for adding/removing video URL input fields.
    -   A `<SessionDetails />` component will display this content to all users with read access, rendering the URLs as clickable links.

## 4. Code Specifications (Key Logic)

### `TrainingSessionService.cs` - Scheduling and Booking Orchestration

```csharp
// Inside TrainingSessionService.cs

// Requires injection of IBookingService, ICurrentUserService, ApplicationDbContext
public async Task<Guid> CreateSessionAsync(CreateSessionRequest request)
{
    await using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        // 1. Check for Coach/Team scheduling conflicts (simplified example)
        var team = await _context.Teams.FindAsync(request.TeamId);
        var coachOrTeamConflict = await _context.TrainingSessions
            .AnyAsync(s => (s.TeamId == request.TeamId || s.Team.CoachId == team.CoachId)
                        && request.StartTime < s.EndTime && request.EndTime > s.StartTime);

        if (coachOrTeamConflict)
        {
            throw new ValidationException("The coach or team has a conflicting event at this time.");
        }

        // 2. Create a booking in the Facility module to reserve the location
        var bookingRequest = new CreateBookingRequest
        {
            BookableResourceId = request.LocationId,
            EventTitle = $"Training: {team.Name}",
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            // Other booking details...
        };
        
        // This call will throw a ConflictException if the location is unavailable,
        // which will be caught by the catch block below.
        Guid bookingId = await _bookingService.CreateBookingAsync(bookingRequest);

        // 3. If booking is successful, create the TrainingSession
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
        // Re-throw as a more generic validation exception for the API layer
        throw new ValidationException($"Location conflict: {ex.Message}");
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### `TrainingSessionService.cs` - Attendance Marking Logic

```csharp
// Inside TrainingSessionService.cs

public async Task MarkAttendanceAsync(Guid sessionId, MarkAttendanceRequest request)
{
    var session = await _context.TrainingSessions.FindAsync(sessionId);
    if (session == null) throw new NotFoundException("Training Session not found.");

    // Authorization: Ensure current user is the coach for this team or a director
    var team = await _context.Teams.FindAsync(session.TeamId);
    if (team.CoachId != _currentUserService.UserId && !_currentUserService.HasPermission("permission:sessions.manage.all"))
    {
        throw new ForbiddenAccessException("You are not authorized to mark attendance for this team.");
    }
    
    // Validation: Prevent marking attendance for future sessions
    if (session.StartTime > DateTime.UtcNow)
    {
        throw new ValidationException("Attendance can only be marked for sessions that have started.");
    }

    foreach(var record in request.AttendanceRecords)
    {
        var attendanceRecord = await _context.TrainingAttendances
            .FirstOrDefaultAsync(a => a.TrainingSessionId == sessionId && a.PlayerId == record.PlayerId);

        if (attendanceRecord == null)
        {
            attendanceRecord = new TrainingAttendance
            {
                TrainingSessionId = sessionId,
                PlayerId = record.PlayerId
            };
            _context.TrainingAttendances.Add(attendanceRecord);
        }

        attendanceRecord.AttendanceStatus = record.Status;
        attendanceRecord.MarkedAt = DateTime.UtcNow;
    }

    await _context.SaveChangesAsync();
}
```