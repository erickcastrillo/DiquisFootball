# Task Context
Define the `ImportJob` entity to track asynchronous import operations. Implement the `DataPortabilityJob` to handle CSV parsing, validation (dry run), and bulk insertion.

# Core References
- **Plan:** [17 - Implementation_Plan_DataPortability.md](./17%20-%20Implementation_Plan_DataPortability.md)
- **Tech Guide:** [UtilityModules_TechnicalGuide.md](../../Technical%20documentation/UtilityModules_TechnicalGuide/UtilityModules_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `ImportJob.cs`:**
    *   Path: `Diquis.Domain/Entities/ImportJob.cs`
    *   Inherit from `BaseEntity`, implement `IMustHaveTenant`.
    *   Properties: `OriginalFilename`, `Status` (Enum), `TotalRowCount`, `ProcessedRowCount`, `ErrorReportUrl`, `TenantId`.
2.  **Define `ImportJobStatus` Enum:**
    *   Pending, Uploading, Validating, FailedValidation, Importing, Completed, Failed.
3.  **Implement `DataPortabilityJob.cs`:**
    *   Path: `Diquis.Infrastructure/BackgroundJobs/DataPortabilityJob.cs`.
    *   Use `CsvHelper`.
    *   Logic:
        *   Read CSV.
        *   Dry Run Validation (Check required fields, formats).
        *   If errors -> Update Status to `FailedValidation`, upload Error Report.
        *   If valid -> Bulk Insert, Update Status to `Completed`.
4.  **Unit Test:**
    *   Create `Diquis.Infrastructure.Tests/DataPortability/ImportValidationTests.cs`.
    *   Verify invalid CSV handling.

# Acceptance Criteria
- [ ] `ImportJob` entity exists.
- [ ] `DataPortabilityJob` handles validation and reporting.
- [ ] Unit tests pass.
