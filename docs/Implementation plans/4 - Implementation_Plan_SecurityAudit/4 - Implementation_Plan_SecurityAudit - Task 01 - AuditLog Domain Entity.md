# Task Context
Define the `AuditLog` entity in the Domain layer. This entity will store immutable records of data modification events (CREATE, UPDATE, DELETE). It must implement `IMustHaveTenant` to ensure audit logs are strictly isolated per tenant. Additionally, define the `[Auditable]` attribute to mark entities that should be tracked.

# Core References
- **Plan:** [4 - Implementation_Plan_SecurityAudit.md](./4%20-%20Implementation_Plan_SecurityAudit.md)
- **Tech Guide:** [InternationalizationSecurity_TechnicalGuide.md](../../Technical%20documentation/InternationalizationSecurity_TechnicalGuide/InternationalizationSecurity_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `AuditLog.cs`:**
    *   Path: `Diquis.Domain/Entities/Common/AuditLog.cs`
    *   Inherit from `BaseEntity` and implement `IMustHaveTenant`.
    *   Properties:
        *   `Timestamp` (DateTime)
        *   `UserId` (Guid?)
        *   `SourceIpAddress` (string?)
        *   `ActionType` (string, required)
        *   `TableName` (string, required)
        *   `RecordId` (string, required, `[Column(TypeName = "jsonb")]`)
        *   `OldValue` (string?, `[Column(TypeName = "jsonb")]`)
        *   `NewValue` (string?, `[Column(TypeName = "jsonb")]`)
        *   `TenantId` (string, required)
2.  **Create `AuditableAttribute.cs`:**
    *   Path: `Diquis.Domain/Entities/Common/AuditableAttribute.cs`
    *   Attribute usage: `[AttributeUsage(AttributeTargets.Class)]`
3.  **Apply Attribute:**
    *   Add `[Auditable]` to `ApplicationUser` in `Diquis.Domain/Entities/Identity/ApplicationUser.cs` (and `Tenant` if available).

# Acceptance Criteria
- [ ] `AuditLog.cs` exists with all properties and interfaces.
- [ ] `AuditableAttribute.cs` exists.
- [ ] `ApplicationUser` is marked as `[Auditable]`.
- [ ] Solution builds successfully.
