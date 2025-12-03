# Task Context
Define the core domain entities for Training Sessions: `TrainingSession` and `TrainingAttendance`. These entities are tenant-specific and must implement `IMustHaveTenant`.

# Core References
- **Plan:** [10 - Implementation_Plan_TrainingSessions.md](./10%20-%20Implementation_Plan_TrainingSessions.md)
- **Tech Guide:** [TrainingSessions_TechnicalGuide.md](../../Technical%20documentation/TrainingSessions_TechnicalGuide/TrainingSessions_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `TrainingSession.cs`:**
    *   Path: `Diquis.Domain/Entities/TrainingSession.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `TeamId` (FK), `StartTime`, `EndTime`, `LocationId` (FK), `BookingId` (FK), `SessionFocus`, `DrillDescription`, `VideoLinks` (jsonb), `Attendances` (Collection), `TenantId`.
2.  **Create `TrainingAttendance.cs`:**
    *   Path: `Diquis.Domain/Entities/TrainingAttendance.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `TrainingSessionId` (FK), `PlayerId` (FK), `AttendanceStatus` (Enum), `MarkedAt`, `TenantId`.
3.  **Define `AttendanceStatus` Enum:**
    *   Create `Diquis.Domain/Enums/AttendanceStatus.cs` (Pending, Present, Absent, Excused).
4.  **Configure Context:**
    *   Ensure `ApplicationDbContext` includes these DbSets.

# Acceptance Criteria
- [ ] `TrainingSession.cs` and `TrainingAttendance.cs` exist.
- [ ] `AttendanceStatus` enum is defined.
- [ ] Entities are correctly configured in DbContext.
