# Task Context
Define the `PlayerChurnRisk` entity and implement the `ChurnPredictionJob`. This background job runs nightly to analyze player data (attendance, financials, engagement) and calculate a risk score.

# Core References
- **Plan:** [21 - Implementation_Plan_AIRevenueGuardian.md](./21%20-%20Implementation_Plan_AIRevenueGuardian.md)
- **Tech Guide:** [MODULE_16_C_AI_ANALYTICS_TechnicalGuide.md](../../Technical%20documentation/MODULE_16_C_AI_ANALYTICS_TechnicalGuide/MODULE_16_C_AI_ANALYTICS_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `PlayerChurnRisk.cs`:**
    *   Path: `Diquis.Domain/Entities/PlayerChurnRisk.cs`.
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `PlayerId`, `RiskScore` (Enum: High, Medium), `TriggeringSignals` (JSON array), `LastCalculatedUtc`, `TenantId`.
2.  **Configure Context:**
    *   Add `DbSet<PlayerChurnRisk>` to `ApplicationDbContext`.
    *   Create migration `AddPlayerChurnRiskEntity`.
3.  **Implement `ChurnPredictionJob.cs`:**
    *   Path: `Diquis.Infrastructure/BackgroundJobs/ChurnPredictionJob.cs`.
    *   Logic:
        *   Iterate active players.
        *   Check Signals:
            *   Attendance < 50% (last 30 days).
            *   Overdue Invoice > 5 days.
            *   Parent Login > 60 days ago.
        *   Calculate Score: 2+ signals = High, 1 signal = Medium.
        *   Update/Insert `PlayerChurnRisk` records.
4.  **Unit Test:**
    *   Create `ChurnPredictionJobTests.cs`.
    *   Verify risk scoring logic.

# Acceptance Criteria
- [ ] `PlayerChurnRisk` entity created and migrated.
- [ ] `ChurnPredictionJob` correctly calculates risk scores based on signals.
- [ ] Unit tests pass.
