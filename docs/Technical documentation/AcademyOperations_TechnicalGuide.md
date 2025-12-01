# Technical Implementation Guide: Academy Operations

This document provides a detailed technical guide for implementing the features outlined in the "Academy Operations" Functional Requirement Specification (FRS) using the Nano ASP.NET Boilerplate.

## 1. Architectural Analysis

### Domain Entities

Based on the FRS, the following core entities are required:

1.  **`InventoryItem`**: Represents a physical asset belonging to an academy.
    *   `Name` (string, required)
    *   `Description` (string, optional)
    *   `Quantity` (int, required)
    *   `Category` (string, e.g., "Balls", "Uniforms")

2.  **`FinancialRecord`**: Represents a single financial transaction within an academy.
    *   `Amount` (decimal, required)
    *   `Date` (DateTime, required)
    *   `Description` (string, optional)
    *   `ReferenceNumber` (string, optional)
    *   `PayerId` (Guid, foreign key to `ApplicationUser`)

3.  **`Tenant`** (existing): The boilerplate's `Tenant` entity will be used. The FRS requirement for an `academy_owner` to update their profile (name, logo) will be implemented by extending the existing tenant management capabilities.

### Multi-Tenancy Scope

-   **`IMustHaveTenant` Required:**
    -   `InventoryItem`: Each item is exclusive to one academy.
    -   `FinancialRecord`: Each transaction is exclusive to one academy.
-   **Shared/Global:**
    -   The `Tenant` entity itself is global but serves as the root for all tenant-specific data.

### Permissions & Authorization

The roles defined in the FRS will be mapped to ASP.NET Core authorization policies. These policies should be registered in the `Diquis.Infrastructure` layer.

| FRS Role | Policy Name | Permissions Claim |
| :--- | :--- | :--- |
| `super_user` | `IsSuperUser` | `permission:tenants.manage` |
| `academy_owner` | `IsAcademyOwner` | `permission:academy.owner` |
| `academy_admin` | `IsAcademyAdmin` | `permission:inventory.manage`, `permission:finance.manage` |
| `director_of_football` | `IsDirector` | `permission:inventory.view` |

## 2. Scaffolding Strategy (CLI)

Execute these commands from the `Diquis.Application/Services` directory to create the required vertical slices.

1.  **Inventory Management:**
    ```bash
    dotnet new nano-service -s InventoryItem -p InventoryItems -ap Diquis
    dotnet new nano-controller -s InventoryItem -p InventoryItems -ap Diquis
    ```

2.  **Financial Records Management:**
    ```bash
    dotnet new nano-service -s FinancialRecord -p FinancialRecords -ap Diquis
    dotnet new nano-controller -s FinancialRecord -p FinancialRecords -ap Diquis
    ```

## 3. Implementation Plan (Agile Breakdown)

### User Story: Tenant & Academy Configuration
**As an** `academy_owner`, **I want to** configure my academy's profile (name, logo), **so that** I can personalize our identity on the platform.

**Technical Tasks:**
1.  **Domain:** The existing `Tenant` entity in `Diquis.Domain` will be used. Confirm it has properties for `Name` and `LogoUrl`.
2.  **Persistence:** No new `DbSet` is needed. A new migration will be required if the `Tenant` entity is modified.
    -   *Migration Command:* `add-migration -Context BaseDbContext -o Persistence/Migrations/BaseDb UpdateTenantWithLogo`
3.  **Application (DTOs):** In `Diquis.Application/Services/Tenants/DTOs`, create:
    -   `UpdateAcademyProfileRequest` with properties for `Name` and `string? LogoUrl`.
    -   Create a separate endpoint/service for logo image uploads which will return a URL.
4.  **Application (Service):** In `TenantManagementService.cs`, add a method `UpdateCurrentAcademyAsync(UpdateAcademyProfileRequest request)`.
    -   This service method will retrieve the `TenantId` from `ICurrentTenantUserService`.
    -   It will update the `Tenant` entity corresponding to the current tenant.
    -   Validation: The academy `Name` must not be null or empty.
5.  **API:** In `TenantsController.cs`, add a `PUT /api/tenants/profile` endpoint.
    -   This endpoint will be secured with the `IsAcademyOwner` policy.
    -   It will call the new `UpdateCurrentAcademyAsync` service method.
    -   Add a `POST /api/tenants/profile/logo` endpoint for the image upload.
6.  **UI (React/Client):**
    -   Create a new page component `src/pages/academy/Settings.tsx`.
    -   This component will contain a form (`<AcademyProfileForm />`) pre-populated with the current tenant's data.
    -   Implement an image upload component that sends the file to the logo endpoint and updates the profile with the returned URL.

### User Story: Inventory Management
**As an** `academy_admin`, **I need to** add, track, and update academy assets, **so that** we can manage our inventory effectively.

**Technical Tasks:**
1.  **Domain:** The auto-generated `InventoryItem.cs` in `Diquis.Domain/Entities` will be used. Ensure it implements `BaseEntity` and `IMustHaveTenant`. Add properties for `Quantity` and `Category`.
2.  **Persistence:** The scaffolding command will add `DbSet<InventoryItem>` to the `ApplicationDbContext`.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddInventoryItemEntity`
3.  **Application (DTOs):** The auto-generated DTOs in `Diquis.Application/Services/InventoryItems/DTOs` will be used. Ensure they include `Quantity` and `Category`.
4.  **Application (Service):** In the generated `InventoryItemService.cs`:
    -   Implement the business logic for creating and updating items.
    -   **Crucial Validation:** Add logic to the `Create/Update` methods to ensure `request.Quantity` is not less than 0.
5.  **API:** In the generated `InventoryItemsController.cs`:
    -   Apply authorization policies to the endpoints:
        -   `POST`, `PUT`, `DELETE`: `[Authorize(Policy = "IsAcademyAdmin")]`
        -   `GET`: `[Authorize(Policy = "IsAcademyAdmin, IsDirector")]` (using a combined policy or multiple attributes).
6.  **UI (React/Client):**
    -   Create `src/pages/inventory/InventoryList.tsx`.
    -   This component will use TanStack Table to display inventory items.
    -   **TanStack Table Columns:** `Name`, `Category`, `Quantity`, `Actions (Edit/Delete)`.
    -   Create `src/components/inventory/InventoryItemForm.tsx` for creating and editing items, including validation to prevent negative quantities.

### User Story: Financial Records Management
**As an** `academy_admin`, **I need to** log payments from parents/players, **so that** I can maintain accurate financial records.

**Technical Tasks:**
1.  **Domain:** The auto-generated `FinancialRecord.cs` in `Diquis.Domain/Entities` will be used. Ensure it implements `BaseEntity` and `IMustHaveTenant`. Add `Amount`, `Date`, `Description`, `ReferenceNumber`, and `PayerId` properties.
2.  **Persistence:** The scaffolding command will add `DbSet<FinancialRecord>` to `ApplicationDbContext`.
    -   *Migration Command:* `add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AddFinancialRecordEntity`
3.  **Application (DTOs):** In `Diquis.Application/Services/FinancialRecords/DTOs`, update the DTOs to include all required fields.
4.  **Application (Service):** In the generated `FinancialRecordService.cs`:
    -   **Crucial Validation:** In the `Create/Update` methods, add logic to ensure `request.Amount` is a positive number.
    -   Add logic to verify that the `PayerId` corresponds to a valid user within the current tenant.
5.  **API:** In the generated `FinancialRecordsController.cs`:
    -   Apply the `[Authorize(Policy = "IsAcademyAdmin")]` policy to all endpoints.
6.  **UI (React/Client):**
    -   Create `src/pages/financials/FinancialsLedger.tsx`.
    -   Use TanStack Table to display a list of all transactions.
    -   **TanStack Table Columns:** `Date`, `Payer`, `Description`, `Amount`, `Actions`.
    -   Create `src/components/financials/LogPaymentForm.tsx`. This form will include a searchable dropdown to select the payer (player/parent).

## 4. Code Specifications (Key Logic)

### `InventoryItemService.cs` - Quantity Validation

```csharp
// Inside the CreateInventoryItemAsync or UpdateInventoryItemAsync method

public async Task<Guid> CreateInventoryItemAsync(CreateInventoryItemRequest request)
{
    // ... AutoMapper mapping ...

    if (request.Quantity < 0)
    {
        throw new FluentValidation.ValidationException("Inventory item quantity cannot be negative.");
    }

    var inventoryItem = new InventoryItem
    {
        Name = request.Name,
        Description = request.Description,
        Quantity = request.Quantity,
        Category = request.Category
        // TenantId is set automatically by the DbContext
    };

    await _context.InventoryItems.AddAsync(inventoryItem);
    await _context.SaveChangesAsync();

    return inventoryItem.Id;
}
```

### `FinancialRecordService.cs` - Amount Validation

```csharp
// Inside the CreateFinancialRecordAsync method

public async Task<Guid> CreateFinancialRecordAsync(CreateFinancialRecordRequest request)
{
    if (request.Amount <= 0)
    {
        throw new FluentValidation.ValidationException("Payment amount must be a positive number.");
    }

    // Verify the payer exists within the tenant
    var payerExists = await _context.Users
        .AnyAsync(u => u.Id == request.PayerId); // The global query filter will scope this to the current tenant

    if (!payerExists)
    {
        throw new NotFoundException("Payer not found in this academy.");
    }
    
    // ... AutoMapper mapping and entity creation ...

    var financialRecord = new FinancialRecord
    {
        Amount = request.Amount,
        Date = request.Date,
        Description = request.Description,
        PayerId = request.PayerId
        // TenantId is set automatically
    };

    await _context.FinancialRecords.AddAsync(financialRecord);
    await _context.SaveChangesAsync();

    return financialRecord.Id;
}
```