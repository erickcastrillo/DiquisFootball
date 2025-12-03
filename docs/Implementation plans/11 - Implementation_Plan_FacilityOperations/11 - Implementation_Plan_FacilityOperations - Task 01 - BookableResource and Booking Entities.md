# Task Context
Define the core domain entities for Facility Operations: `BookableResource` and `Booking`. These entities are tenant-specific and must implement `IMustHaveTenant`. `BookableResource` supports flexible configuration (e.g., for splittable pitches) via a JSON column.

# Core References
- **Plan:** [11 - Implementation_Plan_FacilityOperations.md](./11%20-%20Implementation_Plan_FacilityOperations.md)
- **Tech Guide:** [FacilityResourceOperations_TechnicalGuide.md](../../Technical%20documentation/FacilityResourceOperations_TechnicalGuide/FacilityResourceOperations_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `BookableResource.cs`:**
    *   Path: `Diquis.Domain/Entities/BookableResource.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `Name`, `ResourceType` (Enum), `TotalQuantity`, `Status` (Enum), `Configuration` (jsonb), `TenantId`.
2.  **Create `Booking.cs`:**
    *   Path: `Diquis.Domain/Entities/Booking.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `BookableResourceId` (FK), `BookedById` (FK), `EventTitle`, `StartTime`, `EndTime`, `BookedZone` (string?), `BookedQuantity` (int), `Status` (Enum), `TenantId`.
3.  **Define Enums:**
    *   `ResourceType`: Pitch, Room, Equipment.
    *   `ResourceStatus`: Available, OutOfService.
    *   `BookingStatus`: Confirmed, Pending.
4.  **Configure Context:**
    *   Ensure `ApplicationDbContext` includes these DbSets.

# Acceptance Criteria
- [ ] `BookableResource.cs` and `Booking.cs` exist.
- [ ] Enums are defined.
- [ ] Entities are correctly configured in DbContext.
