# Task Context
Define the `Notification` entity and implement the `NotificationService`. This service acts as a central dispatcher, routing alerts based on user preferences (Email, Push, In-App) and criticality.

# Core References
- **Plan:** [16 - Implementation_Plan_CommunicationEngine.md](./16%20-%20Implementation_Plan_CommunicationEngine.md)
- **Tech Guide:** [UtilityModules_TechnicalGuide.md](../../Technical%20documentation/UtilityModules_TechnicalGuide/UtilityModules_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `Notification.cs`:**
    *   Path: `Diquis.Domain/Entities/Notification.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `UserId` (FK), `Message`, `DeepLink`, `EventType`, `IsRead`, `Criticality` (Enum), `TenantId`.
2.  **Create `UserNotificationPreference.cs`:**
    *   Path: `Diquis.Domain/Entities/UserNotificationPreference.cs`.
    *   Properties: `UserId`, `Preferences` (JSON string).
3.  **Implement `NotificationService.cs`:**
    *   Path: `Diquis.Application/Services/Notifications/NotificationService.cs`.
    *   Implement `SendNotificationAsync`:
        *   Save `Notification` to DB.
        *   Check `Criticality` -> If Critical, force send to all channels.
        *   Check Preferences -> If Digest, stop.
        *   Else, enqueue jobs for preferred channels (Push/Email).
4.  **Unit Test:**
    *   Create `Diquis.Application.Tests/Notifications/NotificationRoutingTests.cs`.
    *   Verify routing logic (Critical vs. Digest vs. Immediate).

# Acceptance Criteria
- [ ] `Notification` and `UserNotificationPreference` entities exist.
- [ ] `NotificationService` implements smart routing logic.
- [ ] Unit tests pass.
