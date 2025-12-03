# Security Audit Logging: Implementation & Testing Plan

## 1. Executive Summary

This document provides a comprehensive implementation and testing plan for the Security Audit Logging module. As a critical non-functional requirement, this module will provide immutable, tenant-scoped audit trails for all significant data modifications, directly addressing the user story: *"As an Academy Owner... I need to know exactly who made the change... and what the old and new values were."*

The implementation leverages a modern Entity Framework Core `SaveChangesInterceptor` for automatic, non-invasive auditing. The plan details the backend architecture, the creation of a user-friendly React modal for viewing audit history, and a robust integration testing strategy to ensure reliability.

## 2. Architectural Blueprint: The `AuditLog` Entity

The foundation of the audit system is the `AuditLog` entity. Based on the technical guide, it will be defined in the `Diquis.Domain` project to capture all necessary details of a data modification event.

**Action:** Create the `AuditLog.cs` entity and a supporting `AuditableAttribute.cs`.

**File:** `Diquis.Domain/Entities/Common/AuditLog.cs`
```csharp
using System.ComponentModel.DataAnnotations.Schema;
using Diquis.Domain.Entities.Multitenancy;

namespace Diquis.Domain.Entities.Common;

/// <summary>
/// Represents an immutable audit trail record for a single data modification event.
/// These logs are tenant-specific.
/// </summary>
[Table("AuditLogs")]
public class AuditLog : BaseEntity, IMustHaveTenant
{
    /// <summary>
    /// The UTC timestamp when the event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The ID of the user who performed the action.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// The IP address from which the action was initiated.
    /// </summary>
    public string? SourceIpAddress { get; set; }

    /// <summary>
    /// The type of action performed (e.g., CREATE, UPDATE, DELETE).
    /// </summary>
    public required string ActionType { get; set; }

    /// <summary>
    /// The name of the database table that was affected.
    /// </summary>
    public required string TableName { get; set; }

    /// <summary>
    /// The primary key(s) of the affected record, serialized as JSON.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required string RecordId { get; set; }

    /// <summary>
    /// A JSON blob representing the entity's state before the change. Null for CREATE actions.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? OldValue { get; set; }

    /// <summary>
    /// A JSON blob representing the entity's state after the change. Null for DELETE actions.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? NewValue { get; set; }
    
    // Foreign key for the IMustHaveTenant interface
    public required string TenantId { get; set; }
}
```

**File:** `Diquis.Domain/Entities/Common/AuditableAttribute.cs`
```csharp
namespace Diquis.Domain.Entities.Common;

/// <summary>
/// Marks an entity to be automatically audited by the AuditSaveChangesInterceptor.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AuditableAttribute : Attribute
{
}
```

**Action:** Mark existing critical entities (e.g., `ApplicationUser`, `Tenant`) with the `[Auditable]` attribute.
```csharp
// In Diquis.Domain/Entities/Identity/ApplicationUser.cs
[Auditable]
public class ApplicationUser : IdentityUser
{
    // ... existing properties
}
```

## 3. Backend Implementation: Automatic Auditing

We will implement a `SaveChangesInterceptor` from Entity Framework Core. This is a clean, modern approach that is more decoupled than overriding `DbContext.SaveChanges` directly. The interceptor will automatically inspect changes and create `AuditLog` entries.

**Action:** Create the interceptor in the `Diquis.Infrastructure` project.

**File:** `Diquis.Infrastructure/Persistence/Interceptors/AuditSaveChangesInterceptor.cs`
```csharp
using Diquis.Application.Common;
using Diquis.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace Diquis.Infrastructure.Persistence.Interceptors;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentTenantUserService _currentUserService;

    public AuditSaveChangesInterceptor(ICurrentTenantUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        GenerateAuditEntries(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        GenerateAuditEntries(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void GenerateAuditEntries(DbContext? context)
    {
        if (context == null) return;

        context.ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            // Only audit entities marked with [Auditable]
            if (entry.Entity.GetType().GetCustomAttribute<AuditableAttribute>() == null)
                continue;

            var log = new AuditLog
            {
                TableName = entry.Metadata.GetTableName()!,
                UserId = _currentUserService.UserId,
                SourceIpAddress = _currentUserService.IpAddress,
                Timestamp = DateTime.UtcNow,
                TenantId = _currentUserService.TenantId!,
                ActionType = entry.State.ToString().ToUpper(),
                RecordId = JsonSerializer.Serialize(entry.Properties.Where(p => p.Metadata.IsPrimaryKey()).ToDictionary(p => p.Metadata.Name, p => p.CurrentValue)),
                OldValue = entry.State is EntityState.Modified or EntityState.Deleted ? JsonSerializer.Serialize(entry.OriginalValues.ToObject()) : null,
                NewValue = entry.State is EntityState.Added or EntityState.Modified ? JsonSerializer.Serialize(entry.CurrentValues.ToObject()) : null,
            };
            auditEntries.Add(log);
        }

        if (auditEntries.Any())
        {
            context.Set<AuditLog>().AddRange(auditEntries);
        }
    }
}
```
**Action:** Register the interceptor in `Program.cs` or the persistence DI extensions.
```csharp
// In DI configuration for persistence
services.AddSingleton<AuditSaveChangesInterceptor>();

services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<AuditSaveChangesInterceptor>();
    options.UseNpgsql(...) 
           .AddInterceptors(interceptor);
});
```

## 4. Frontend Extension (React)

To display the audit history, we'll create a reusable modal component and integrate it into existing management tables.

### 4.1. API Endpoint for Audit History

First, an API endpoint is needed to fetch the logs for a specific record.

**Service & Controller (Conceptual):**
-   **Service:** `IAuditLogService` with a method `GetAuditHistoryAsync(string tableName, string recordId)`.
-   **Controller:** `AuditLogsController` with an endpoint:
    `GET /api/audit-logs/{tableName}/{recordId}`

### 4.2. Add "View History" Button to Tables

**Action:** In components like `UserManagementTable.tsx`, add a new column with a button to trigger the modal.

```tsx
// In a component like /src/pages/users/UserManagementTable.tsx

// ... inside the table column definitions ...
{
  Header: 'Actions',
  accessor: 'actions',
  Cell: ({ row }) => (
    <>
      {/* ... other action buttons ... */}
      <Button variant="outline-secondary" size="sm" onClick={() => handleViewHistory(row.original.id)}>
        <i className="bi bi-clock-history"></i> {/* Example icon */}
      </Button>
    </>
  ),
}

// ... in the component's main body ...
const [historyModalShow, setHistoryModalShow] = useState(false);
const [selectedRecord, setSelectedRecord] = useState({ id: '', table: '' });

const handleViewHistory = (recordId: string) => {
  setSelectedRecord({ id: recordId, table: 'AspNetUsers' }); // Hardcode table name for this view
  setHistoryModalShow(true);
};

// ... in the JSX return ...
<AuditHistoryModal
  show={historyModalShow}
  onHide={() => setHistoryModalShow(false)}
  tableName={selectedRecord.table}
  recordId={selectedRecord.id}
/>
```

### 4.3. Create `AuditHistoryModal` Component

**Action:** Create the modal component to fetch and display the logs.

**File:** `src/components/auditing/AuditHistoryModal.tsx`
```tsx
import { useEffect, useState } from 'react';
import { Modal, Button, Spinner, ListGroup, Badge } from 'react-bootstrap';
import { useAuditApi } from '@/api/auditApi'; // Custom hook for the API call
import ReactJson from 'react-json-view'; // A library for viewing JSON diffs

interface AuditHistoryModalProps {
  show: boolean;
  onHide: () => void;
  tableName: string;
  recordId: string;
}

export const AuditHistoryModal = ({ show, onHide, tableName, recordId }: AuditHistoryModalProps) => {
  const [logs, setLogs] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const { getAuditHistory } = useAuditApi();

  useEffect(() => {
    if (show && recordId) {
      setIsLoading(true);
      getAuditHistory(tableName, recordId)
        .then(data => setLogs(data))
        .finally(() => setIsLoading(false));
    }
  }, [show, recordId, tableName, getAuditHistory]);

  return (
    <Modal show={show} onHide={onHide} size="xl" centered>
      <Modal.Header closeButton>
        <Modal.Title>Audit History for Record: {recordId}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {isLoading ? (
          <Spinner animation="border" />
        ) : (
          <ListGroup>
            {logs.map(log => (
              <ListGroup.Item key={log.id}>
                <div className="d-flex justify-content-between">
                  <strong>{log.actionType} by {log.userId}</strong>
                  <small>{new Date(log.timestamp).toLocaleString()}</small>
                </div>
                <div className="mt-2">
                  {log.oldValue && log.newValue ? (
                    <ReactJson src={{ oldValue: log.oldValue, newValue: log.newValue }} name={false} displayDataTypes={false} theme="monokai" collapsed={true} />
                  ) : (
                     <ReactJson src={log.newValue || log.oldValue} name={false} displayDataTypes={false} theme="monokai" collapsed={true} />
                  )}
                </div>
              </ListGroup.Item>
            ))}
          </ListGroup>
        )}
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onHide}>Close</Button>
      </Modal.Footer>
    </Modal>
  );
};
```

## 5. Testing Strategy

The core of the testing strategy is to ensure that the interceptor correctly and automatically creates logs upon data changes.

### 5.1. Backend Integration Test

**Action:** Create an integration test using an in-memory database or a test container.

**File:** `Diquis.Infrastructure.Tests/Persistence/AuditingTests.cs`
```csharp
using Diquis.Domain.Entities.Identity;
using Diquis.Infrastructure.Persistence;
using Diquis.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Diquis.Infrastructure.Tests.Persistence;

[TestFixture]
public class AuditingTests
{
    private ApplicationDbContext _context;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        // Mock the user service
        var mockUserService = new Mock<ICurrentTenantUserService>();
        mockUserService.Setup(s => s.UserId).Returns(Guid.NewGuid());
        mockUserService.Setup(s => s.TenantId).Returns("test-tenant");

        services.AddSingleton(mockUserService.Object);
        services.AddSingleton<AuditSaveChangesInterceptor>();
        
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditSaveChangesInterceptor>();
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                   .AddInterceptors(interceptor);
        });

        var sp = services.BuildServiceProvider();
        _context = sp.GetRequiredService<ApplicationDbContext>();
    }

    [Test]
    public async Task When_AuditableEntity_IsModified_Should_CreateAuditLog()
    {
        // Arrange: Create and save an initial entity
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "testuser", Email = "test@test.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // Act: Modify the entity
        var userToUpdate = await _context.Users.FindAsync(user.Id);
        userToUpdate!.UserName = "updateduser";
        await _context.SaveChangesAsync();

        // Assert
        var auditLogs = await _context.AuditLogs.ToListAsync();
        
        // Should have two logs: one for CREATE, one for UPDATE
        auditLogs.Should().HaveCount(2);

        var updateLog = auditLogs.Single(l => l.ActionType == "UPDATE");
        updateLog.TableName.Should().Be("AspNetUsers");
        updateLog.OldValue.Should().Contain("\"UserName\":\"testuser\"");
        updateLog.NewValue.Should().Contain("\"UserName\":\"updateduser\"");
    }
}
```

### 5.2. Frontend Component Test

The `AuditHistoryModal` will be tested using Vitest/Jest and a mocking library like Mock Service Worker (MSW) to simulate the API.

**Scenario:**
1.  **Render the component:** Render `<AuditHistoryModal show={true} ... />`.
2.  **Mock the API:** Intercept the `GET /api/audit-logs/...` request.
3.  **Test Loading State:** Initially, assert that a `<Spinner>` component is visible.
4.  **Test Data Display:** Once the mock API responds, assert that the list items are rendered with the correct data (Action Type, User, Timestamp).
5.  **Test `onHide`:** Simulate a click on the "Close" button and verify the `onHide` mock function was called.

```