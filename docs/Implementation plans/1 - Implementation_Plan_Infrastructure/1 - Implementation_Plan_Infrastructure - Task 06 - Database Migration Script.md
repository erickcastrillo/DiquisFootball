# Task Context
Develop a PowerShell script to manage database schema updates across the multi-tenant fleet. The script must fetch connection strings for all tenants and execute EF Core migrations in parallel to ensure efficient updates.

# Core References
- **Plan:** [1 - Implementation_Plan_Infrastructure.md](./1%20-%20Implementation_Plan_Infrastructure.md)
- **Tech Guide:** [CommercialOnboarding_TechnicalGuide.md](../../Technical%20documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md)
- **FRS:** [CommercialOnboarding_FRS.md](../../Business%20Requirements/CommercialOnboarding_FRS/CommercialOnboarding_FRS.md)

# Step-by-Step Instructions
1.  **Create Script:** Create `scripts/run-migrations.ps1`.
2.  **Parameters:** Accept `ManagementDbConnectionString` and `MaxParallelJobs`.
3.  **Fetch Tenants:** Query the Management Database (BaseDbContext) to get connection strings for all Professional and Enterprise tenants.
4.  **Parallel Execution:**
    *   Use `ForEach-Object -Parallel` (PowerShell 7+ feature).
    *   Inside the loop, execute `dotnet ef database update --connection $ConnectionString`.
5.  **Error Handling:** Wrap execution in `try/catch` and log failures to a file.

# Acceptance Criteria
- [ ] File `scripts/run-migrations.ps1` exists.
- [ ] Script queries for tenant connection strings.
- [ ] Script executes migrations in parallel.
- [ ] Error handling and logging are implemented.
