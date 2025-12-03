# Task Context
Implement the `PredictiveAnalyticsService` to expose risk data and generate AI-powered re-engagement scripts. Also includes the Cash Flow Forecast logic.

# Core References
- **Plan:** [21 - Implementation_Plan_AIRevenueGuardian.md](./21%20-%20Implementation_Plan_AIRevenueGuardian.md)
- **Tech Guide:** [MODULE_16_C_AI_ANALYTICS_TechnicalGuide.md](../../Technical%20documentation/MODULE_16_C_AI_ANALYTICS_TechnicalGuide/MODULE_16_C_AI_ANALYTICS_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `PredictiveAnalyticsService.cs`:**
    *   Path: `Diquis.Application/Services/PredictiveAnalytics/PredictiveAnalyticsService.cs`.
    *   Inject `IPlayerChurnRiskRepository`, `IFinancialRecordRepository`, `IOpenAIService`.
2.  **Implement `GetPlayersAtRiskAsync`:**
    *   Return list of players with `RiskScore` > Low.
3.  **Implement `GenerateReEngagementScriptAsync`:**
    *   Fetch Player/Parent/Owner names.
    *   Construct empathetic prompt.
    *   Call AI Service.
4.  **Implement `GetCashFlowForecastAsync`:**
    *   Fetch historical financial data (12 months).
    *   Generate 90-day projection (placeholder or simple algorithm).
    *   Generate AI Insight Summary.

# Acceptance Criteria
- [ ] Service exposes risk data.
- [ ] AI generates empathetic scripts.
- [ ] Cash flow forecast returns historical + projected data.
