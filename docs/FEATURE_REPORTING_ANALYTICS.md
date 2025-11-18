# Diquis - Reporting & Analytics Feature

## Overview

The Reporting & Analytics module provides comprehensive business intelligence capabilities for football academies. This system generates actionable insights across all aspects of academy operations, from financial performance to player development analytics.

## Core Functionality

### Report Categories

#### 1. Financial Reports
- **Revenue Analysis**: Income tracking by source.
- **Expense Tracking**: Operating costs, equipment, salaries.
- **Profit & Loss Statements**: Monthly, quarterly, and annual reports.
- **Budget vs. Actual**: Variance analysis.

#### 2. Player Analytics
- **Development Tracking**: Individual player progress over time.
- **Performance Metrics**: Skill assessments, attendance, improvement rates.
- **Player Retention**: Dropout analysis and retention strategies.

#### 3. Team Performance Reports
- **Team Statistics**: Performance metrics by team.
- **Attendance Reports**: Training and match attendance patterns.
- **Coaching Effectiveness**: Player improvement under different coaches.

#### 4. Operational Reports
- **Facility Utilization**: Field usage and scheduling efficiency.
- **Equipment & Asset Reports**: Asset utilization, maintenance costs.
- **Staff Performance**: Coach ratings and productivity.

#### 5. Business Intelligence
- **Academy Benchmarking**: Performance comparison across academies.
- **Growth Projections**: Enrollment and revenue forecasts.
- **KPI Dashboards**: Key performance indicators and trends.

## Data Model (Conceptual)

This is a conceptual data model. The implementation will use C# entity classes in the `Diquis.Domain` layer.

### Report Entity
```csharp
public class Report : BaseEntity
{
    public Guid AcademyId { get; set; }
    public string Name { get; set; }
    public ReportType ReportType { get; set; }
    public string QueryTemplate { get; set; } // Could be JSON or another format
    public ReportStatus Status { get; set; }
    public Guid CreatedByUserId { get; set; }
}
```

### ReportGeneration Entity
```csharp
public class ReportGeneration : BaseEntity
{
    public Guid ReportId { get; set; }
    public Guid RequestedByUserId { get; set; }
    public DateTime DateRangeStart { get; set; }
    public DateTime DateRangeEnd { get; set; }
    public ReportFormat Format { get; set; }
    public GenerationStatus Status { get; set; }
    public string FileUrl { get; set; }
    public long FileSizeBytes { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
```

### FinancialTransaction Entity
```csharp
public class FinancialTransaction : BaseEntity
{
    public Guid AcademyId { get; set; }
    public TransactionType TransactionType { get; set; }
    public string Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public Guid? PlayerId { get; set; }
    public Guid? TeamId { get; set; }
}
```

### PlayerMetric Entity
```csharp
public class PlayerMetric : BaseEntity
{
    public Guid AcademyId { get; set; }
    public Guid PlayerId { get; set; }
    public string MetricType { get; set; } // e.g., "attendance_rate", "skill_improvement"
    public decimal MetricValue { get; set; }
    public DateTime MetricDate { get; set; }
}
```

## API Endpoints (Conceptual)

### Reports API
- `GET /api/v1/{academyId}/reports`: List available reports.
- `POST /api/v1/{academyId}/reports/{reportId}/generate`: Asynchronously generate a report.
- `GET /api/v1/{academyId}/reports/generations/{generationId}`: Check the status of a report generation.
- `GET /api/v1/{academyId}/reports/generations/{generationId}/download`: Download a completed report.

### Analytics API
- `GET /api/v1/{academyId}/analytics/revenue`: Get revenue summary.
- `GET /api/v1/{academyId}/analytics/player-development`: Get player development analytics.

## Business Logic (Application Layer)

The `Diquis.Application` project will contain services to handle the core logic:

- **ReportGenerationService**: Manages the asynchronous creation of reports. It will queue a background job, generate the report file (PDF, Excel, etc.), upload it to storage, and notify the user.
- **FinancialAnalyticsService**: Provides methods to calculate and retrieve financial metrics like P&L, revenue by category, etc.
- **PlayerAnalyticsService**: Provides methods for analyzing player data, such as performance trends and retention rates.
- **BusinessIntelligenceService**: Handles more complex analytics, like KPI dashboards and cross-academy benchmarking.

## Background Jobs

A background job processor like **Hangfire** or **Quartz.NET** will be used for:

- **ReportGenerationJob**: A long-running job that generates the report file.
- **ScheduledReportJob**: A recurring job that automatically generates and distributes subscribed reports.
- **MetricsCalculationJob**: A periodic job to aggregate data and calculate key metrics for dashboards.

## Implementation Priority

### Phase 1: Core Financial Reporting
- Basic income/expense tracking.
- Simple financial reports (P&L, revenue summary).
- Asynchronous report generation and download.

### Phase 2: Player Analytics
- Player development tracking.
- Attendance analysis.
- Performance metrics calculation.

### Phase 3: Advanced Analytics & BI
- Business intelligence dashboards.
- Predictive analytics for retention.
- Multi-academy benchmarking.
- Automated insights and recommendations.
