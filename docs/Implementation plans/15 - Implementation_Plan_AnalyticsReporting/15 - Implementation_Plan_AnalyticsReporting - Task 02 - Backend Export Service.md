# Task Context
Implement the backend capability to export reports as CSV or PDF files. This involves creating an export service and controller endpoints.

# Core References
- **Plan:** [15 - Implementation_Plan_AnalyticsReporting.md](./15%20-%20Implementation_Plan_AnalyticsReporting.md)

# Step-by-Step Instructions
1.  **Create `IExcelExportService` / `IPdfExportService`:**
    *   Define interfaces for exporting data.
2.  **Implement Export Services:**
    *   `CsvExportService`: Use a library like `CsvHelper` or manual string building.
    *   `PdfExportService`: Use a library like `QuestPDF` or `DinkToPdf` (if available/preferred, otherwise mock/stub for now).
3.  **Add Report Endpoints to `AnalyticsController`:**
    *   `GET /api/analytics/reports/financial-summary-csv`:
        *   Fetch data via `AnalyticsService`.
        *   Convert to CSV bytes.
        *   Return `FileResult` (text/csv).
    *   Secure with `IsAcademyAdmin` policy.

# Acceptance Criteria
- [ ] Export interfaces and implementations exist.
- [ ] Controller endpoints return file downloads.
- [ ] Security policies are applied.
