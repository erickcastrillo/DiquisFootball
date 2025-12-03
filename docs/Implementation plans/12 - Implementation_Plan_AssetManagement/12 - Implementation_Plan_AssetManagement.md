# Asset Management: Implementation & Testing Plan

## 1. Executive Summary

This document provides a detailed implementation and testing plan for the Asset Management module. This system is designed to provide academy administrators with a robust tool for tracking physical inventory, from initial stock intake to individual assignment to players, supported by a full audit trail for all quantity changes.

The plan outlines the domain model for assets and audit entries, the backend logic for inventory tracking and transactional updates, the frontend implementation of a color-coded inventory grid, and a testing strategy to validate the critical stock deduction logic.

## 2. Architectural Blueprint: Domain Entities

To accurately model both bulk inventory and specific player assignments, we will introduce three core tenant-specific entities.

**Action:** Create the following entity files in the `Diquis.Domain/Entities/` directory.

**File:** `Diquis.Domain/Entities/Asset.cs`
```csharp
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;

namespace Diquis.Domain.Entities;

/// <summary>
/// Represents a type of asset in the academy's inventory (e.g., "Home Jersey - Size M").
/// </summary>
public class Asset : BaseEntity, IMustHaveTenant
{
    public required string Name { get; set; }
    public string? Category { get; set; } // e.g., "Uniform", "Training Equipment"
    public int TotalQuantity { get; set; } // The total number of this item owned by the academy.
    public required string TenantId { get; set; }
}
```

**File:** `Diquis.Domain/Entities/PlayerAsset.cs`
```csharp
namespace Diquis.Domain.Entities;

/// <summary>
/// A linking entity representing a specific instance of an Asset assigned to a Player.
/// </summary>
public class PlayerAsset : BaseEntity, IMustHaveTenant
{
    public Guid AssetId { get; set; }
    public Asset Asset { get; set; }

    public Guid PlayerId { get; set; }
    public ApplicationUser Player { get; set; }

    /// <summary>
    /// Specific details of the assigned item, e.g., "10" for a jersey number.
    /// </summary>
    public string? AssignedIdentifier { get; set; } 
    public DateTime DateAssigned { get; set; }
    public required string TenantId { get; set; }
}
```

**File:** `Diquis.Domain/Entities/AssetAuditEntry.cs`
```csharp
namespace Diquis.Domain.Entities;

/// <summary>
/// An immutable log entry tracking every change to an Asset's quantity.
/// </summary>
public class AssetAuditEntry : BaseEntity, IMustHaveTenant
{
    public Guid AssetId { get; set; }
    public Asset Asset { get; set; }

    public Guid UserId { get; set; } // User who performed the action.
    public AuditChangeType ChangeType { get; set; } // Enum: InitialStock, Assignment, Return, Correction, Retired
    public int QuantityChange { get; set; } // e.g., +50, -1
    public int NewTotalQuantity { get; set; }
    public string? Reason { get; set; }
    public required string TenantId { get; set; }
}

public enum AuditChangeType { InitialStock, Assignment, Return, Correction, Retired }
```

## 3. Backend Implementation: Inventory Tracking & Auditing

The backend service will be responsible for calculating available stock and ensuring all quantity changes are audited within a single transaction.

### 3.1. Inventory Tracking & DTO

The `AvailableQuantity` is a calculated field, not stored in the database. It will be computed in the service layer and returned in a DTO.

**DTO:** `Diquis.Application/Services/Assets/DTOs/AssetDto.cs`
```csharp
public class AssetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int TotalQuantity { get; set; }
    public int AssignedQuantity { get; set; }
    public int AvailableQuantity => TotalQuantity - AssignedQuantity;
}
```
The service method `GetAssetsAsync` will use a projection to populate this DTO, calculating `AssignedQuantity` with a subquery: `_context.PlayerAssets.Count(pa => pa.AssetId == asset.Id)`.

### 3.2. Transactional Auditing Logic

Assigning an asset to a player must be an atomic operation that creates the assignment record and logs the stock change.

**Action:** Implement the `AssignAssetToPlayerAsync` and a private `LogAuditAsync` method in `AssetService.cs`.

**File:** `Diquis.Application/Services/Assets/AssetService.cs` (enhancement)
```csharp
public class AssetService : IAssetService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentTenantUserService _currentUser;
    // ...

    public async Task AssignAssetToPlayerAsync(Guid assetId, Guid playerId, string identifier)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null) throw new NotFoundException("Asset not found.");

        var assignedCount = await _context.PlayerAssets.CountAsync(pa => pa.AssetId == assetId);
        if (assignedCount >= asset.TotalQuantity)
        {
            throw new ValidationException("Asset is out of stock and cannot be assigned.");
        }

        // 1. Create the assignment record
        var playerAsset = new PlayerAsset
        {
            AssetId = assetId,
            PlayerId = playerId,
            AssignedIdentifier = identifier,
            DateAssigned = DateTime.UtcNow
        };
        _context.PlayerAssets.Add(playerAsset);
        
        // 2. Log the audit entry
        await LogAuditAsync(assetId, AuditChangeType.Assignment, -1, $"Assigned to player {playerId}");

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private async Task LogAuditAsync(Guid assetId, AuditChangeType type, int quantityChange, string reason)
    {
        var asset = await _context.Assets.FindAsync(assetId); // Re-fetch for current state
        var auditEntry = new AssetAuditEntry
        {
            AssetId = assetId,
            UserId = _currentUser.UserId,
            ChangeType = type,
            QuantityChange = quantityChange,
            NewTotalQuantity = asset.TotalQuantity, // Assuming this method is called after TotalQuantity is updated if needed
            Reason = reason
        };
        _context.AssetAuditEntries.Add(auditEntry);
    }
}
```

## 4. Frontend Implementation (React)

A new feature folder will contain all UI for asset management, centered around a data grid for inventory visualization.

### 4.1. Folder Structure

**Action:** Create the new feature folder `src/features/assets`.

### 4.2. Inventory Grid with Color-Coded Stock Levels

The main view will use `TanStack Table` to display the inventory, with conditional styling to highlight low-stock items.

**File:** `src/features/assets/components/InventoryGrid.tsx`
```tsx
import { useMemo } from 'react';
import { useReactTable, getCoreRowModel, flexRender } from '@tanstack/react-table';
import { useAssetsApi } from '../hooks/useAssetsApi';

const LOW_STOCK_THRESHOLD = 10;

export const InventoryGrid = () => {
  const { data: assets = [] } = useAssetsApi().useGetAssets();

  const columns = useMemo(() => [
    { header: 'Asset Name', accessorKey: 'name' },
    { header: 'Category', accessorKey: 'category' },
    { header: 'Total', accessorKey: 'totalQuantity' },
    { header: 'Assigned', accessorKey: 'assignedQuantity' },
    {
      header: 'Available',
      accessorKey: 'availableQuantity',
      // CRUCIAL: Cell renderer for conditional color-coding
      cell: ({ getValue }) => {
        const available = getValue();
        const isLowStock = available <= LOW_STOCK_THRESHOLD;
        const stockColor = available === 0 ? 'text-danger' : isLowStock ? 'text-warning' : '';
        
        return <strong className={stockColor}>{available}</strong>;
      },
    },
    // ... Actions column with "Assign" / "Adjust Stock" buttons
  ], []);

  const table = useReactTable({ data: assets, columns, getCoreRowModel: getCoreRowModel() });

  return (
    // ... TanStack Table JSX rendering logic ...
    <table>
      {/* ... table headers ... */}
      <tbody>
        {table.getRowModel().rows.map(row => (
          <tr key={row.id}>
            {row.getVisibleCells().map(cell => (
              <td key={cell.id}>
                {flexRender(cell.column.columnDef.cell, cell.getContext())}
              </td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
};
```

## 5. Testing Strategy

The testing strategy will focus on the most critical business rule: ensuring that stock deduction and auditing upon asset assignment are correct and atomic.

### 5.1. Backend Integration Test: Stock Deduction Logic

This test will validate the entire transaction of assigning an asset, confirming that the inventory count is correct and that the action was audited.

**Action:** Create an integration test in `Diquis.Infrastructure.Tests`.

**File:** `Diquis.Infrastructure.Tests/Assets/AssetAssignmentTests.cs`
```csharp
using FluentAssertions;
using NUnit.Framework;

[TestFixture]
public class AssetAssignmentTests
{
    private AssetService _service;
    private ApplicationDbContext _context;

    [SetUp]
    public async Task Setup()
    {
        // Use an in-memory database
        _context = /* ... get in-memory context ... */;
        var mockUserService = new Mock<ICurrentTenantUserService>();
        mockUserService.Setup(s => s.UserId).Returns(Guid.NewGuid());

        _service = new AssetService(_context, mockUserService.Object);

        // ARRANGE: Seed the database with an asset and a player
        var asset = new Asset { Id = Guid.NewGuid(), Name = "Size M Jersey", TotalQuantity = 10 };
        var player = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testplayer" };
        _context.Assets.Add(asset);
        _context.Users.Add(player);
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task AssignAssetToPlayerAsync_WhenStockIsAvailable_ShouldCreateAssignmentAndAuditEntry()
    {
        // ARRANGE
        var asset = await _context.Assets.FirstAsync();
        var player = await _context.Users.FirstAsync();

        // ACT
        await _service.AssignAssetToPlayerAsync(asset.Id, player.Id, "Jersey #10");

        // ASSERT
        // 1. Verify the PlayerAsset link was created
        var playerAsset = await _context.PlayerAssets.FirstOrDefaultAsync();
        playerAsset.Should().NotBeNull();
        playerAsset.AssetId.Should().Be(asset.Id);
        playerAsset.PlayerId.Should().Be(player.Id);

        // 2. Verify the AssetAuditEntry was created
        var auditEntry = await _context.AssetAuditEntries.FirstOrDefaultAsync();
        auditEntry.Should().NotBeNull();
        auditEntry.AssetId.Should().Be(asset.Id);
        auditEntry.ChangeType.Should().Be(AuditChangeType.Assignment);
        auditEntry.QuantityChange.Should().Be(-1);

        // 3. Verify calculated Available Quantity is correct (most important)
        var assetDto = await _service.GetAssetDtoByIdAsync(asset.Id); // Assuming this method exists
        assetDto.AvailableQuantity.Should().Be(9);
    }
    
    [Test]
    public async Task AssignAssetToPlayerAsync_WhenStockIsZero_ShouldThrowValidationException()
    {
        // ARRANGE
        var asset = await _context.Assets.FirstAsync();
        asset.TotalQuantity = 0; // Adjust stock for the test case
        await _context.SaveChangesAsync();
        var player = await _context.Users.FirstAsync();

        // ACT
        Func<Task> act = () => _service.AssignAssetToPlayerAsync(asset.Id, player.Id, "Jersey #10");

        // ASSERT
        await act.Should().ThrowAsync<ValidationException>().WithMessage("Asset is out of stock*");
    }
}
```
This test suite ensures the atomicity and correctness of the stock management logic, confirming that an assignment correctly decrements available inventory and is fully audited.
