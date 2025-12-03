# Task Context
Implement the backend logic for Analytics & Reporting. This module is read-only and relies on DTOs and optimized queries rather than new domain entities.

# Core References
- **Plan:** [15 - Implementation_Plan_AnalyticsReporting.md](./15%20-%20Implementation_Plan_AnalyticsReporting.md)
- **Tech Guide:** [AnalyticsReporting_TechnicalGuide.md](../../Technical%20documentation/AnalyticsReporting_TechnicalGuide/AnalyticsReporting_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create DTOs:**
    *   Path: `Diquis.Application/Services/Analytics/DTOs/`.
    *   `AcademyOwnerDashboardDto`: TotalPlayerCount, TeamsCount, RevenueThisMonth, SessionsThisWeek.
    *   `CoachDashboardDto`: TeamAttendanceRatePercentage, UpcomingMatchesCount, TopPerformingPlayers (List<PlayerMetricDto>).
    *   `PlayerMetricDto`: PlayerId, PlayerName, MetricValue.
2.  **Create `AnalyticsService.cs`:**
    *   Path: `Diquis.Application/Services/Analytics/AnalyticsService.cs`.
    *   Implement `IAnalyticsService`.
    *   `GetAcademyOwnerDashboardAsync`: Query `Users`, `Teams`, `FinancialRecords`, `TrainingSessions`.
    *   `GetCoachDashboardAsync`: Query `TrainingAttendance`, `Matches`.
3.  **Create `AnalyticsController.cs`:**
    *   Path: `Diquis.WebApi/Controllers/AnalyticsController.cs`.
    *   Endpoint: `GET /api/analytics/dashboard`.
    *   Logic: Check user role (Owner/Coach) and call appropriate service method.

# Acceptance Criteria
- [ ] DTOs are defined.
- [ ] `AnalyticsService` implements optimized queries.
- [ ] `AnalyticsController` routes based on role.
