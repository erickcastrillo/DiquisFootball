# Task Context
Implement the backend business logic for scheduling training sessions and marking attendance. This involves conflict detection (internal and external via `IBookingService`) and transactional updates.

# Core References
- **Plan:** [10 - Implementation_Plan_TrainingSessions.md](./10%20-%20Implementation_Plan_TrainingSessions.md)

# Step-by-Step Instructions
1.  **Implement `CreateSessionAsync`:**
    *   Input: `CreateSessionRequest`.
    *   Logic:
        *   Start Transaction.
        *   Check internal conflicts (Team/Coach overlap).
        *   Call `_bookingService.CreateBookingAsync` (External conflict check).
        *   Create `TrainingSession` with `BookingId`.
        *   Commit Transaction.
        *   Handle `ConflictException` from booking service.
2.  **Implement `MarkAttendanceAsync`:**
    *   Input: `sessionId`, `MarkAttendanceRequest`.
    *   Logic:
        *   Authorize (Coach/Director).
        *   Validate (Session started?).
        *   Upsert `TrainingAttendance` records.
3.  **Unit Test:**
    *   Create `Diquis.Infrastructure.Tests/Training/SchedulingConflictTests.cs`.
    *   Verify conflict detection logic (mocking `IBookingService`).

# Acceptance Criteria
- [ ] `TrainingSessionService` implements scheduling and attendance logic.
- [ ] Conflict detection works for both internal and external conflicts.
- [ ] Unit tests pass.
