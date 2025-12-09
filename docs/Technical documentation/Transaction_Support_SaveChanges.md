# Transaction Support for SaveChanges Operations

## Overview
Enhanced the `SaveChangesAsync` operations in both `BaseDbContext` and `ApplicationDbContext` to include explicit transaction support with automatic rollback on errors.

## Changes Made

### 1. OnSaveChangesExtensions.cs
- **Added using**: `Microsoft.EntityFrameworkCore.Storage` for transaction support
- **New Method**: `SaveChangesWithTransactionAsync<TContext>`
  - Wraps save operations in an explicit database transaction
  - Automatically commits on success
  - Automatically rolls back on any exception
  - Detects and reuses existing transactions to avoid nesting issues
  - Uses `await using` for proper async disposal (.NET 10)

### 2. BaseDbContext.cs
- Updated `SaveChangesAsync` to use `SaveChangesWithTransactionAsync`
- Added documentation about automatic rollback behavior

### 3. ApplicationDbContext.cs
- Updated `SaveChangesAsync` to use `SaveChangesWithTransactionAsync`
- Ensures tenant-specific data changes are transactional

## Benefits

### Atomicity
All changes in a single `SaveChangesAsync` call are now guaranteed to be atomic:
- Either all changes succeed and are committed
- Or all changes fail and are rolled back
- No partial updates in case of errors

### Error Handling
```csharp
try 
{
    await context.SaveChangesAsync();
    // All changes committed successfully
}
catch (Exception ex)
{
    // All changes have been automatically rolled back
    // Database is in the state before SaveChangesAsync was called
}
```

### Nested Transaction Safety
The implementation checks for existing transactions:
```csharp
var currentTransaction = context.Database.CurrentTransaction;
if (currentTransaction != null)
{
    // Reuse existing transaction
}
```

This prevents issues when:
- Calling SaveChanges from within a manually managed transaction
- Multiple SaveChanges calls in a single scope
- Background jobs that manage their own transactions

## Usage Examples

### Standard Usage (No Changes Required)
```csharp
// Existing code works exactly the same
await context.SaveChangesAsync();
```

### With Manual Transaction
```csharp
// You can still manage transactions manually
using var transaction = await context.Database.BeginTransactionAsync();
try
{
    // Multiple save operations
    await context.SaveChangesAsync(); // Reuses transaction
    await context.SaveChangesAsync(); // Reuses transaction
    
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

### In Background Jobs (e.g., ProvisionTenantJob)
```csharp
// Transaction automatically protects tenant provisioning
try
{
    var user = new ApplicationUser { /* ... */ };
    context.Users.Add(user);
    await context.SaveChangesAsync(); // Auto-rollback on error
}
catch (Exception ex)
{
    // Database is unchanged, no orphaned records
    _logger.LogError(ex, "Failed to provision tenant");
}
```

## Original Issue Fixed

### Problem
Empty or null `CurrentUserId` caused `FormatException` when calling `Guid.Parse("")`

### Solution
Changed validation from:
```csharp
CurrentUserId != null ? Guid.Parse(CurrentUserId) : Guid.Empty
```

To:
```csharp
!string.IsNullOrWhiteSpace(CurrentUserId) ? Guid.Parse(CurrentUserId) : Guid.Empty
```

This properly handles:
- `null` values
- Empty strings (`""`)
- Whitespace-only strings (`"   "`)

## Testing Recommendations

### Unit Tests
1. Test SaveChanges with invalid GUID formats
2. Test SaveChanges with null/empty CurrentUserId
3. Test transaction rollback on exceptions
4. Test nested transaction scenarios

### Integration Tests
1. Verify tenant provisioning rolls back on user creation failure
2. Test multi-entity updates with partial failures
3. Verify soft delete transactions
4. Test concurrent save operations

## Performance Considerations

### Minimal Overhead
- Transaction creation is lightweight in PostgreSQL
- Reuses existing transactions when possible
- `await using` ensures proper disposal

### When Transactions Are Created
```csharp
// New transaction created (none exists)
await context.SaveChangesAsync();

// Transaction reused (already exists)
using var tx = await context.Database.BeginTransactionAsync();
await context.SaveChangesAsync(); // No new transaction
await tx.CommitAsync();
```

## Related Files
- `Diquis.Infrastructure/Persistence/Extensions/OnSaveChangesExtensions.cs`
- `Diquis.Infrastructure/Persistence/Contexts/BaseDbContext.cs`
- `Diquis.Infrastructure/Persistence/Contexts/ApplicationDbContext.cs`
- `Diquis.Infrastructure/BackgroundJobs/ProvisionTenantJob.cs`

## Date
January 2025 - .NET 10 Implementation
