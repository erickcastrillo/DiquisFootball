# SaaS Back-Office (Analytics Extension): Implementation & Testing Plan

## 1. Executive Summary

This document outlines the implementation and testing strategy for extending the SaaS Back-Office, a critical component for internal business intelligence and operational support. The plan details the creation of a global analytics dashboard for `System Super-Admins`, providing key metrics like Total MRR and tenant distribution.

The plan covers the architectural blueprint for cross-tenant data aggregation, the frontend implementation of a revenue chart and a "Login As Tenant" feature, and a security-focused testing strategy to ensure strict role separation and prevent data leakage to unauthorized users.

## 2. Architectural Blueprint: Global Dashboard Aggregates

To provide a global view of the platform's health, we need to perform data aggregations that span across all tenants. This logic must be carefully isolated to a new, dedicated service that operates outside the standard tenant-scoped data context.

### 2.1. Data Transfer Object (DTO)

First, we define the data contract for our new dashboard endpoint.

**File:** `Diquis.Application/Services/SaaSAnalytics/GlobalAnalyticsDto.cs`
```csharp
namespace Diquis.Application.Services.SaaSAnalytics;

public class GlobalAnalyticsDto
{
    /// <summary>
    /// Total calculated Monthly Recurring Revenue across all active tenants.
    /// </summary>
    public decimal TotalMRR { get; set; }

    /// <summary>
    /// The total number of tenants with an 'Active' status.
    /// </summary>
    public int ActiveTenants { get; set; }

    /// <summary>
    /// The number of new tenants that were created in the current month.
    /// </summary>
    public int NewTenantsThisMonth { get; set; }

    /// <summary>
    /// A dictionary showing the count of tenants per subscription tier.
    /// </summary>
    public Dictionary<string, int> TierDistribution { get; set; } = new();
}
```

### 2.2. Service & Controller Implementation

A new service will be created to query the `BaseDbContext` (the management database), which contains the list of all tenants and their subscription tiers.

**Action:** Create a new `ISaasAnalyticsService` and its implementation. This service **must not** depend on `ApplicationDbContext`.

**File:** `Diquis.Application/Services/SaaSAnalytics/SaasAnalyticsService.cs`
```csharp
public class SaasAnalyticsService : ISaasAnalyticsService
{
    private readonly BaseDbContext _managementContext;
    private readonly IConfiguration _config;

    public SaasAnalyticsService(BaseDbContext managementContext, IConfiguration config)
    {
        _managementContext = managementContext;
        _config = config;
    }

    public async Task<GlobalAnalyticsDto> GetGlobalAnalyticsAsync()
    {
        var activeTenants = await _managementContext.Tenants
            .Where(t => t.SubscriptionStatus == SubscriptionStatus.Active)
            .ToListAsync();
            
        var tierPrices = _config.GetSection("SubscriptionPrices").Get<Dictionary<string, decimal>>() 
            ?? new Dictionary<string, decimal>();

        var totalMrr = activeTenants.Sum(t => tierPrices.GetValueOrDefault(t.SubscriptionTier.ToString(), 0m));

        var tierDistribution = activeTenants
            .GroupBy(t => t.SubscriptionTier.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return new GlobalAnalyticsDto
        {
            TotalMRR = totalMrr,
            ActiveTenants = activeTenants.Count,
            NewTenantsThisMonth = activeTenants.Count(t => t.CreatedAt.Month == DateTime.UtcNow.Month && t.CreatedAt.Year == DateTime.UtcNow.Year),
            TierDistribution = tierDistribution
        };
    }
}
```

**Action:** Create a new `SaaSAnalyticsController` secured for `System Super-Admins` only.

```csharp
[ApiController]
[Route("api/saas-analytics")]
[Authorize(Roles = "SystemSuperAdmin")] // CRITICAL: This endpoint is for super admins only.
public class SaaSAnalyticsController : ControllerBase
{
    private readonly ISaasAnalyticsService _analyticsService;

    public SaaSAnalyticsController(ISaasAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("global")]
    public async Task<IActionResult> GetGlobalAnalytics()
    {
        var analytics = await _analyticsService.GetGlobalAnalyticsAsync();
        return Ok(analytics);
    }
}
```

## 3. Frontend Extension (React)

The existing Super Admin Dashboard will be enhanced with new data visualizations and operational tools.

### 3.1. `GlobalRevenueChart` Component

**Action:** Install `Chart.js` and its React wrapper.
```bash
npm install chart.js react-chartjs-2
```

**Action:** Create the chart component to visualize tenant distribution.

**File:** `src/components/admin/GlobalRevenueChart.tsx`
```tsx
import { useEffect, useState } from 'react';
import { Bar } from 'react-chartjs-2';
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js';
import { useSassAnalyticsApi } from '@/api/sassAnalyticsApi'; // Custom hook for the API

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

export const GlobalRevenueChart = () => {
  const [chartData, setChartData] = useState<any>({ labels: [], datasets: [] });
  const { getGlobalAnalytics } = useSassAnalyticsApi();

  useEffect(() => {
    getGlobalAnalytics().then(data => {
      const labels = Object.keys(data.tierDistribution);
      const values = Object.values(data.tierDistribution);

      setChartData({
        labels,
        datasets: [{
          label: '# of Tenants',
          data: values,
          backgroundColor: 'rgba(54, 162, 235, 0.6)',
        }],
      });
    });
  }, [getGlobalAnalytics]);

  return (
    <div>
      <h3>Tenant Distribution by Tier</h3>
      <Bar data={chartData} />
    </div>
  );
};
```

### 3.2. "Login As Tenant" (Impersonation) Feature

This powerful feature allows an admin to debug issues by temporarily assuming the identity of a tenant's user.

**Backend Action:** Create a secure API endpoint for impersonation.
- **Endpoint:** `POST /api/identity/impersonate/{tenantId}`
- **Security:** Must be protected by the `SystemSuperAdmin` role.
- **Logic:**
    1.  Validates the caller is a super admin.
    2.  Finds the primary `Academy Owner` for the target `tenantId`.
    3.  Generates a **new JWT** for that user.
    4.  **Crucially,** adds two extra claims to the new JWT:
        -   `impersonator_id`: The ID of the super admin who initiated the action.
        -   `original_token`: The super admin's original JWT, allowing them to switch back.
    5.  Returns the new impersonation JWT to the client.

**Frontend Action:** Add the button and logic to the tenant list.

```tsx
// In a component like /src/pages/admin/TenantList.tsx

// ... inside the table column definitions ...
{
  Header: 'Actions',
  Cell: ({ row }) => (
    <Button variant="warning" size="sm" onClick={() => handleLoginAs(row.original.id)}>
      Login As Tenant
    </Button>
  ),
}

// ... logic in the component ...
const { impersonateTenant } = useIdentityApi(); // Custom API hook

const handleLoginAs = async (tenantId: string) => {
  if (window.confirm('You are about to impersonate this tenant. Proceed?')) {
    try {
      const { impersonationToken } = await impersonateTenant(tenantId);
      
      // Store the new token and force a reload to re-initialize the app
      localStorage.setItem('jwt', impersonationToken);
      window.location.href = '/dashboard'; // Redirect to tenant dashboard
    } catch (error) {
      console.error('Failed to impersonate tenant', error);
    }
  }
};
```

## 4. Testing Strategy

The highest priority is testing the strict role-based access control to prevent any tenant-level user from accessing the global back-office data.

### 4.1. Backend Security Integration Test

This test ensures the API endpoint for global analytics is correctly secured.

**Action:** Create an integration test in `Diquis.WebApi.Tests`.

**File:** `Diquis.WebApi.Tests/Security/SaaSAnalyticsSecurityTests.cs`
```csharp
using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using NUnit.Framework;

namespace Diquis.WebApi.Tests.Security;

[TestFixture]
public class SaaSAnalyticsSecurityTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SaaSAnalyticsSecurityTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Test]
    public async Task GetGlobalAnalytics_WithSuperAdminToken_ShouldReturnOk()
    {
        // Arrange
        var token = await TestAuthHelper.GetSuperAdminTokenAsync(); // Helper to generate a valid admin token
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/saas-analytics/global");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetGlobalAnalytics_WithTenantAdminToken_ShouldReturnForbidden()
    {
        // Arrange
        var token = await TestAuthHelper.GetTenantAdminTokenAsync(); // Helper to generate a non-admin token
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/saas-analytics/global");

        // Assert
        // CRITICAL: Must be Forbidden, not Unauthorized. It proves the user is valid but lacks the role.
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
```

### 4.2. Frontend Conditional Rendering Test

This test ensures that the sensitive dashboard components are never rendered in the DOM for non-admin users.

**Action:** Create a component test using Vitest/Jest and React Testing Library.

**File:** `src/pages/admin/AdminDashboard.test.tsx`
```tsx
import { render, screen } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { AdminDashboard } from './AdminDashboard';
import { useAuth } from '@/hooks/useAuth'; // Mockable auth hook

// Mock the auth hook
vi.mock('@/hooks/useAuth');
const mockedUseAuth = vi.mocked(useAuth);

// Mock the child component to test for its presence
vi.mock('@/components/admin/GlobalRevenueChart', () => ({
  GlobalRevenueChart: () => <div data-testid="global-revenue-chart"></div>,
}));

describe('AdminDashboard', () => {
  it('should render GlobalRevenueChart for a super admin', () => {
    // Arrange
    mockedUseAuth.mockReturnValue({ user: { roles: ['SystemSuperAdmin'] } });

    // Act
    render(<AdminDashboard />);

    // Assert
    expect(screen.getByTestId('global-revenue-chart')).toBeInTheDocument();
  });

  it('should NOT render GlobalRevenueChart for a regular tenant admin', () => {
    // Arrange
    mockedUseAuth.mockReturnValue({ user: { roles: ['AcademyOwner'] } });

    // Act
    render(<AdminDashboard />);

    // Assert
    expect(screen.queryByTestId('global-revenue-chart')).not.toBeInTheDocument();
  });
});
```
This multi-layered testing approach validates both the API security and the UI presentation layer, ensuring that back-office features are exclusively available to authorized personnel.
