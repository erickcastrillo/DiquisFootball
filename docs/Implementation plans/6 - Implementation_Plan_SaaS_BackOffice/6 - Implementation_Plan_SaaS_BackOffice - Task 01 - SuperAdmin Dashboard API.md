# Task Context
Implement the backend API for the Super Admin Dashboard. This involves creating a service to aggregate global metrics (Total MRR, Active Tenants, etc.) from the `BaseDbContext` and exposing them via a secure controller accessible only to `SystemSuperAdmin` users.

# Core References
- **Plan:** [6 - Implementation_Plan_SaaS_BackOffice.md](./6%20-%20Implementation_Plan_SaaS_BackOffice.md)

# Step-by-Step Instructions
1.  **Create DTO:**
    *   Path: `Diquis.Application/Services/SaaSAnalytics/GlobalAnalyticsDto.cs`
    *   Properties: `TotalMRR`, `ActiveTenants`, `NewTenantsThisMonth`, `TierDistribution`.
2.  **Create Service Interface & Implementation:**
    *   `ISaasAnalyticsService` and `SaasAnalyticsService`.
    *   Inject `BaseDbContext` and `IConfiguration`.
    *   Implement `GetGlobalAnalyticsAsync`:
        *   Query `Tenants` table.
        *   Calculate MRR based on config prices.
        *   Aggregate counts.
3.  **Create Controller:**
    *   Path: `Diquis.WebApi/Controllers/SaaSAnalyticsController.cs`
    *   Attribute: `[Authorize(Roles = "SystemSuperAdmin")]`.
    *   Endpoint: `GET /api/saas-analytics/global`.

# Acceptance Criteria
- [ ] `GlobalAnalyticsDto` exists.
- [ ] `SaasAnalyticsService` correctly calculates metrics.
- [ ] `SaaSAnalyticsController` is secured and returns the DTO.
- [ ] Integration test confirms 403 Forbidden for non-admins.
