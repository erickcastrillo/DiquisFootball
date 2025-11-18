# Diquis - Asset Management Feature

## Overview

The Asset Management module provides comprehensive tracking and management of physical assets owned by the academy, including equipment, uniforms, training materials, and facility resources. This system enables academies to maintain inventory control, track asset allocation, monitor maintenance schedules, and generate asset-related reports.

## Core Functionality

### Asset Categories

- **Equipment**: Balls, cones, goals, nets, training aids
- **Uniforms & Apparel**: Jerseys, shorts, socks, training gear, goalkeeper kits
- **Training Materials**: Bibs, markers, agility ladders, hurdles
- **Facility Assets**: Field maintenance equipment, benches, scoreboards
- **Technology**: Tablets, cameras, timing equipment, heart rate monitors

### Key Features

#### 1. Asset Registration & Cataloging

- **Unique Asset Identification**: Each asset gets a unique ID/barcode.
- **Asset Details**: Name, description, category, brand, model, serial number.
- **Financial Information**: Purchase price, purchase date, vendor, warranty info.
- **Condition Tracking**: New, good, fair, poor, needs repair, retired.
- **Photo Documentation**: Multiple images per asset.
- **Location Assignment**: Current location/storage area.

#### 2. Inventory Management

- **Stock Levels**: Track quantities for consumable items.
- **Reorder Points**: Automatic alerts when stock runs low.
- **Bulk Operations**: Add/update multiple similar items.
- **Asset Valuation**: Current value calculations with depreciation.
- **Audit Trail**: Complete history of asset changes.

#### 3. Asset Allocation & Check-out

- **Player Assignment**: Assign uniforms and personal equipment to players.
- **Team Allocation**: Assign training equipment to specific teams.
- **Coach/Staff Assignment**: Assign tablets, keys, specialized equipment.
- **Temporary Loans**: Short-term equipment checkout system.
- **Return Tracking**: Monitor overdue returns.

#### 4. Maintenance & Lifecycle Management

- **Maintenance Schedules**: Preventive maintenance calendars.
- **Repair Tracking**: Log repairs, costs, and service providers.
- **Replacement Planning**: Track asset lifecycle and replacement needs.
- **Warranty Management**: Monitor warranty status and claims.
- **Disposal Records**: Track asset retirement and disposal.

#### 5. Reporting & Analytics

- **Asset Reports**: Comprehensive asset listings and valuations.
- **Utilization Reports**: Track usage patterns and efficiency.
- **Financial Reports**: Asset depreciation, maintenance costs, ROI.
- **Compliance Reports**: Regulatory compliance and safety inspections.
- **Custom Reports**: Flexible reporting with multiple filters.

## Data Model (Conceptual)

This is a conceptual data model. The implementation will use C# entity classes in the `Diquis.Domain` layer.

### Asset Entity

```csharp
public class Asset : BaseEntity
{
    public Guid AcademyId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid AssetCategoryId { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string SerialNumber { get; set; }
    public string Barcode { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Vendor { get; set; }
    public DateTime? WarrantyExpiresAt { get; set; }
    public AssetCondition Condition { get; set; }
    public string Location { get; set; }
    public bool IsActive { get; set; }

    // Navigation Properties
    public AssetCategory Category { get; set; }
    public ICollection<AssetAllocation> Allocations { get; set; }
    public ICollection<AssetMaintenance> MaintenanceRecords { get; set; }
}
```

### AssetCategory Entity

```csharp
public class AssetCategory : BaseEntity
{
    public Guid? AcademyId { get; set; } // Null for global categories
    public string Name { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public decimal? DepreciationRate { get; set; }
    public int? ExpectedLifespanYears { get; set; }
    public bool IsConsumable { get; set; }
}
```

### AssetAllocation Entity

```csharp
public class AssetAllocation : BaseEntity
{
    public Guid AssetId { get; set; }
    public Guid AllocatableId { get; set; } // e.g., PlayerId, TeamId
    public string AllocatableType { get; set; } // "Player", "Team"
    public DateTime AllocatedAt { get; set; }
    public DateTime? ExpectedReturnAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public Guid AllocatedByUserId { get; set; }
}
```

### AssetMaintenance Entity

```csharp
public class AssetMaintenance : BaseEntity
{
    public Guid AssetId { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public string Description { get; set; }
    public DateTime PerformedAt { get; set; }
    public decimal Cost { get; set; }
    public Guid PerformedByUserId { get; set; }
}
```

## API Endpoints (Conceptual)

- `GET /api/v1/{academyId}/assets`
- `GET /api/v1/{academyId}/assets/{id}`
- `POST /api/v1/{academyId}/assets`
- `PUT /api/v1/{academyId}/assets/{id}`
- `POST /api/v1/{academyId}/assets/{id}/allocate`
- `POST /api/v1/{academyId}/assets/{id}/return`
- `GET /api/v1/{academyId}/assets/{id}/maintenance`
- `POST /api/v1/{academyId}/assets/{id}/maintenance`

## Business Logic (Application Layer)

The `Diquis.Application` project will contain services to handle the core logic:

- **AssetManagementService**: Handles asset creation, updates, and valuation.
- **AssetAllocationService**: Manages the check-out and check-in workflow, including availability validation and overdue notifications.
- **AssetMaintenanceService**: Manages maintenance scheduling and cost tracking.
- **InventoryManagementService**: Handles stock levels for consumable items and reorder alerts.

## Background Jobs

- **AssetMaintenanceReminderJob**: Sends notifications for upcoming scheduled maintenance.
- **AssetReturnReminderJob**: Sends notifications for overdue asset returns.
- **AssetDepreciationJob**: Periodically calculates and updates the depreciated value of assets.

## Integration Points

- **Player Management**: Assign uniforms and personal equipment to players.
- **Team Management**: Allocate training equipment to teams.
- **Reporting Module**: Provide data for asset valuation, utilization, and maintenance cost reports.

## Implementation Priority

### Phase 1: Core Asset Management
- Basic asset registration and tracking.
- Simple allocation system.
- Basic reporting.

### Phase 2: Advanced Features
- Maintenance scheduling.
- Inventory management for consumables.
- Advanced reporting and analytics.

### Phase 3: Integration & Automation
- Barcode/QR code scanning support.
- Automated depreciation calculations.
- Advanced workflow automation.
