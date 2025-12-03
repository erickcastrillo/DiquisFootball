# Analytics & Reporting: Implementation & Testing Plan

## 1. Executive Summary

This document outlines the implementation and testing strategy for the Analytics & Reporting module. This module focuses on aggregating existing data from across the platform to provide role-specific insights and visualizations. It is a read-only module, designed for performance and clear presentation of data.

The plan defines the DTO-based architecture for data aggregation, the backend implementation of performance-optimized queries, the creation of a new analytics feature area in the React frontend with charting components, and a testing strategy focused on validating the accuracy of the aggregated data.

## 2. Architectural Blueprint: Aggregation DTOs

This module does not introduce new domain entities. Instead, it relies on creating Data Transfer Objects (DTOs) on-the-fly to represent aggregated data for dashboards and reports. This approach ensures the analytics module remains decoupled from the core domain.

**Action:** Define role-specific DTOs in a new `Diquis.Application/Services/Analytics/DTOs` folder.

**File:** `Diquis.Application/Services/Analytics/DTOs/AcademyOwnerDashboardDto.cs`
```csharp
namespace Diquis.Application.Services.Analytics.DTOs;

/// <summary>
/// Data contract for the Academy Owner's main dashboard.
/// </summary>
public class AcademyOwnerDashboardDto
{
    public int TotalPlayerCount { get; set; }
    public int TeamsCount { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public int SessionsThisWeek { get; set; }
}
```

**File:** `Diquis.Application/Services/Analytics/DTOs/CoachDashboardDto.cs`
```csharp
namespace Diquis.Application.Services.Analytics.DTOs;

/// <summary>
/// Data contract for the Coach's main dashboard.
/// </summary>
public class CoachDashboardDto
{
    public double TeamAttendanceRatePercentage { get; set; }
    public int UpcomingMatchesCount { get; set; }
    public List<PlayerMetricDto> TopPerformingPlayers { get; set; } = new();
}

public class PlayerMetricDto 
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; }
    public double MetricValue { get; set; }
}
```

## 3. Backend Implementation: Optimized Reporting Queries

The backend service will be responsible for executing efficient, read-only queries against the database to generate the aggregation DTOs. Performance is key, so queries will be designed to be translatable to efficient SQL.

**Action:** Create a new, non-CRUD `AnalyticsService` to house the query logic.

**File:** `Diquis.Application/Services/Analytics/AnalyticsService.cs`
```csharp
public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentTenantUserService _currentUser;

    public AnalyticsService(ApplicationDbContext context, ICurrentTenantUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<AcademyOwnerDashboardDto> GetAcademyOwnerDashboardAsync()
    {
        // Tenant scope is applied automatically by EF Core global query filters.
        var today = DateTime.UtcNow;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);

        // These queries are designed to be translated into efficient SQL COUNT/SUM operations.
        var totalPlayerCount = await _context.Users
            .CountAsync(u => u.UserRoles.Any(ur => ur.Role.Name == "Player"));
            
        var teamsCount = await _context.Teams.CountAsync();

        var revenueThisMonth = await _context.FinancialRecords
            .Where(fr => fr.Date >= startOfMonth)
            .SumAsync(fr => fr.Amount);

        var sessionsThisWeek = await _context.TrainingSessions
            .CountAsync(ts => ts.StartTime >= startOfWeek);

        return new AcademyOwnerDashboardDto
        {
            TotalPlayerCount = totalPlayerCount,
            TeamsCount = teamsCount,
            RevenueThisMonth = revenueThisMonth,
            SessionsThisWeek = sessionsThisWeek
        };
    }
}
```
The `AnalyticsController` will then expose this data via a single, role-aware endpoint `GET /api/analytics/dashboard`, as specified in the technical guide.

## 4. Frontend Implementation (React)

A new feature folder will contain all UI for data visualization.

### 4.1. Folder Structure & Setup

**Action:** Create the folder `src/features/analytics` and install a charting library.
```bash
npm install chart.js react-chartjs-2
```

### 4.2. Dashboard Visualizations

The frontend will use reusable chart and widget components to display the aggregated data from the backend.

**Action:** Create a generic `DataWidget` and a specific `OwnerDashboard` component.

**File:** `src/features/analytics/components/DataWidget.tsx`
```tsx
import { Card } from 'react-bootstrap';

interface DataWidgetProps {
  title: string;
  value: string | number;
  icon: string;
}

export const DataWidget = ({ title, value, icon }: DataWidgetProps) => (
  <Card>
    <Card.Body className="d-flex align-items-center">
      <div className={`fs-1 text-primary me-3 bi ${icon}`}></div>
      <div>
        <h5 className="text-muted">{title}</h5>
        <h2 className="fw-bold">{value}</h2>
      </div>
    </Card.Body>
  </Card>
);
```

**File:** `src/features/analytics/components/OwnerDashboard.tsx`
```tsx
import { Row, Col } from 'react-bootstrap';
import { useAnalyticsApi } from '../hooks/useAnalyticsApi';
import { DataWidget } from './DataWidget';
// import { RevenueChart } from './RevenueChart';

export const OwnerDashboard = () => {
  const { data, isLoading } = useAnalyticsApi().useGetDashboard();

  if (isLoading) return <p>Loading dashboard...</p>;

  return (
    <Row>
      <Col md={3}>
        <DataWidget title="Total Players" value={data.totalPlayerCount} icon="bi-people-fill" />
      </Col>
      <Col md={3}>
        <DataWidget title="Revenue This Month" value={`$${data.revenueThisMonth.toFixed(2)}`} icon="bi-cash-coin" />
      </Col>
      <Col md={3}>
        <DataWidget title="Active Teams" value={data.teamsCount} icon="bi-diagram-3-fill" />
      </Col>
      <Col md={3}>
        <DataWidget title="Sessions This Week" value={data.sessionsThisWeek} icon="bi-calendar-event" />
      </Col>
      {/* <Col md={12} className="mt-4">
        <RevenueChart />
      </Col> */}
    </Row>
  );
};
```

## 5. Testing Strategy

The primary goal of testing is to ensure the accuracy of the aggregated data presented in the reports and dashboards.

### 5.1. Backend Integration Test: Data Accuracy Validation

This test will seed a database with a known, fixed set of data and then run the analytics service against it, asserting that the calculated totals are exactly correct.

**Action:** Create an integration test for the `AnalyticsService`.

**File:** `Diquis.Application.Tests/Analytics/DashboardAccuracyTests.cs`
```csharp
using FluentAssertions;
using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;

[TestFixture]
public class DashboardAccuracyTests
{
    private AnalyticsService _service;
    private ApplicationDbContext _context;

    [SetUp]
    public async Task Setup()
    {
        // ARRANGE: Seed an in-memory database with a known state
        _context = /* ... get in-memory context ... */;
        
        // Add 5 players
        _context.Users.AddRange(TestSeed.GetPlayers(5));
        
        // Add 2 teams
        _context.Teams.AddRange(TestSeed.GetTeams(2));
        
        // Add 3 financial records for this month totaling $1500
        _context.FinancialRecords.AddRange(
            new FinancialRecord { Amount = 500, Date = DateTime.UtcNow.AddDays(-1) },
            new FinancialRecord { Amount = 700, Date = DateTime.UtcNow.AddDays(-2) },
            new FinancialRecord { Amount = 300, Date = DateTime.UtcNow.AddDays(-3) }
        );
        
        // Add 1 financial record from last month
        _context.FinancialRecords.Add(new FinancialRecord { Amount = 1000, Date = DateTime.UtcNow.AddMonths(-1) });

        await _context.SaveChangesAsync();
        
        var mockUserService = new Mock<ICurrentTenantUserService>();
        _service = new AnalyticsService(_context, mockUserService.Object);
    }

    [Test]
    public async Task GetAcademyOwnerDashboardAsync_WithSeededData_ShouldReturnCorrectAggregates()
    {
        // ACT
        var dashboardData = await _service.GetAcademyOwnerDashboardAsync();

        // ASSERT
        // Verify each metric against the known state of the seeded data.
        dashboardData.Should().NotBeNull();
        dashboardData.TotalPlayerCount.Should().Be(5);
        dashboardData.TeamsCount.Should().Be(2);
        dashboardData.RevenueThisMonth.Should().Be(1500); // 500 + 700 + 300
    }
}
```
This test provides high confidence that the backend queries are correctly aggregating data and respecting filters like date ranges, ensuring the data shown to administrators is accurate and reliable.