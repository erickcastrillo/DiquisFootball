# Task Context
Implement the backend service layer for Financial Records. This includes generating the basic CRUD scaffolding and then enhancing the `FinancialRecordService` with specific business logic validation (positive amounts, valid payer) and a summary endpoint.

# Core References
- **Plan:** [7 - Implementation_Plan_AcademyOperations.md](./7%20-%20Implementation_Plan_AcademyOperations.md)

# Step-by-Step Instructions
1.  **Scaffold Services:**
    *   Run `dotnet new nano-service -s FinancialRecord -p FinancialRecords -ap Diquis` (if CLI available, otherwise manually create).
    *   Run `dotnet new nano-controller -s FinancialRecord -p FinancialRecords -ap Diquis`.
2.  **Enhance `FinancialRecordService.cs`:**
    *   Implement `CreateFinancialRecordAsync`:
        *   Validate `request.Amount > 0`.
        *   Validate `PayerId` exists in `_context.Users`.
        *   Save entity.
    *   Implement `GetFinancialSummaryAsync`:
        *   Return `FinancialSummaryDto` (TotalRevenue, TransactionsThisMonth, RevenueThisMonth).
3.  **Enhance `FinancialRecordsController.cs`:**
    *   Add `[HttpGet("summary")]` endpoint calling the service.
    *   Ensure `[Authorize(Policy = "IsAcademyAdmin")]` is applied.

# Acceptance Criteria
- [ ] Service and Controller files exist.
- [ ] Validation logic prevents negative amounts and invalid payers.
- [ ] Summary endpoint returns aggregated data.
