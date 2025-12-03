# Academy Operations (Financials): Implementation & Testing Plan

## 1. Executive Summary

This document details the implementation and testing strategy for the "Academy Operations" module, with a specific focus on financial record management. This module is a core component of the tenant-level administration, providing academy owners and admins with the tools to manage inventory and log financial transactions.

The plan outlines the required domain entities, the backend services for CRUD operations with critical validation logic, the creation of a new "financials" feature area in the React frontend, and a focused testing strategy for payment validation rules.

## 2. Architectural Blueprint: Domain Entities

Following the technical guide, we will define two core domain entities, `FinancialRecord` and `InventoryItem`. Both will be tenant-specific, ensuring strict data isolation.

**Action:** Create the following entity files in the `Diquis.Domain/Entities/` directory.

**File:** `Diquis.Domain/Entities/FinancialRecord.cs`
```csharp
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;

namespace Diquis.Domain.Entities;

/// <summary>
/// Represents a single financial transaction (e.g., a payment received) within an academy.
/// This entity is scoped to a specific tenant.
/// </summary>
public class FinancialRecord : BaseEntity, IMustHaveTenant
{
    /// <summary>
    /// The monetary value of the transaction. Must be positive.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The date the transaction occurred.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// A short description of the transaction (e.g., "Monthly Fee - January").
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// An optional reference number (e.g., Invoice #123, Check #456).
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// The ID of the user (player, parent) who made the payment.
    /// </summary>
    public Guid PayerId { get; set; }

    // Required by the IMustHaveTenant interface for data isolation.
    public required string TenantId { get; set; }
}
```

**File:** `Diquis.Domain/Entities/InventoryItem.cs`
```csharp
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;

namespace Diquis.Domain.Entities;

/// <summary>
/// Represents a physical asset owned by an academy (e.g., balls, cones, uniforms).
/// This entity is scoped to a specific tenant.
/// </summary>
public class InventoryItem : BaseEntity, IMustHaveTenant
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public string? Category { get; set; }
    public required string TenantId { get; set; }
}
```

## 3. Backend Implementation

We will leverage the boilerplate's scaffolding tools to generate the basic vertical slice for `FinancialRecord` and then enhance it with custom business logic and a summary endpoint for the dashboard.

**Action:**
1.  From the `Diquis.Application/Services` directory, run the scaffolding commands:
    ```bash
    dotnet new nano-service -s FinancialRecord -p FinancialRecords -ap Diquis
    dotnet new nano-controller -s FinancialRecord -p FinancialRecords -ap Diquis
    ```
2.  Implement the custom validation logic and the summary endpoint.

### 3.1. Service Layer Validation

The `FinancialRecordService` must be updated to enforce the business rules defined in the FRS.

**File:** `Diquis.Application/Services/FinancialRecords/FinancialRecordService.cs` (enhancement)
```csharp
public class FinancialRecordService : IFinancialRecordService
{
    private readonly ApplicationDbContext _context;
    // ... other injected services

    public async Task<Guid> CreateFinancialRecordAsync(CreateFinancialRecordRequest request)
    {
        // 1. CRUCIAL: Business rule validation
        if (request.Amount <= 0)
        {
            throw new FluentValidation.ValidationException("Payment amount must be a positive number.");
        }

        var payerExists = await _context.Users.AnyAsync(u => u.Id == request.PayerId.ToString());
        if (!payerExists)
        {
            throw new NotFoundException("Payer not found in this academy.");
        }

        // 2. Map DTO to Entity and save
        var financialRecord = new FinancialRecord
        {
            Amount = request.Amount,
            Date = request.Date,
            Description = request.Description,
            PayerId = request.PayerId,
            // TenantId is set automatically by the DbContext/Interceptor
        };

        await _context.FinancialRecords.AddAsync(financialRecord);
        await _context.SaveChangesAsync();

        return financialRecord.Id;
    }

    // ... other CRUD methods
}
```

### 3.2. Financial Summary Endpoint

To power the frontend dashboard, we will create an endpoint that returns aggregated financial data.

**DTO:** `Diquis.Application/Services/FinancialRecords/DTOs/FinancialSummaryDto.cs`
```csharp
public class FinancialSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public int TransactionsThisMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
}
```

**Service Method:** `FinancialRecordService.cs`
```csharp
public async Task<FinancialSummaryDto> GetFinancialSummaryAsync()
{
    var now = DateTime.UtcNow;
    var records = await _context.FinancialRecords.ToListAsync(); // Already scoped to tenant

    return new FinancialSummaryDto
    {
        TotalRevenue = records.Sum(r => r.Amount),
        TransactionsThisMonth = records.Count(r => r.Date.Month == now.Month && r.Date.Year == now.Year),
        RevenueThisMonth = records
            .Where(r => r.Date.Month == now.Month && r.Date.Year == now.Year)
            .Sum(r => r.Amount),
    };
}
```

**Controller Endpoint:** `FinancialRecordsController.cs`
```csharp
[HttpGet("summary")]
[Authorize(Policy = "IsAcademyAdmin")]
public async Task<IActionResult> GetSummary()
{
    var summary = await _financialRecordService.GetFinancialSummaryAsync();
    return Ok(summary);
}
```

## 4. Frontend Implementation (React)

We will create a new feature folder to encapsulate all UI components related to financial management.

### 4.1. Folder Structure

**Action:** Create the new feature folder `src/features/financials` with the following structure:
```
/src/features/financials/
├── components/
│   ├── FinancialDashboard.tsx
│   ├── LogPaymentForm.tsx
│   └── SummaryCard.tsx
├── hooks/
│   └── useFinancialsApi.ts
└── pages/
    └── FinancialsPage.tsx
```

### 4.2. Financial Dashboard & Summary Cards

The dashboard will provide an at-a-glance view of the academy's financial health.

**File:** `src/features/financials/components/SummaryCard.tsx`
```tsx
import { Card } from 'react-bootstrap';

interface SummaryCardProps {
  title: string;
  value: string | number;
  icon: string; // e.g., 'bi bi-cash-coin'
}

export const SummaryCard = ({ title, value, icon }: SummaryCardProps) => (
  <Card>
    <Card.Body>
      <div className="d-flex align-items-center">
        <div className="fs-1 text-primary me-3"><i className={icon}></i></div>
        <div>
          <Card.Title>{title}</Card.Title>
          <Card.Text className="fs-4 fw-bold">{value}</Card.Text>
        </div>
      </div>
    </Card.Body>
  </Card>
);
```

**File:** `src/features/financials/components/FinancialDashboard.tsx`
```tsx
// Fetches summary data and displays it using SummaryCard components
// ... (implementation would fetch from `/api/financial-records/summary` and render cards)
```

### 4.3. Payment Form with Formik

**Action:** Implement the `LogPaymentForm` using Formik for state management and Yup for validation.

**File:** `src/features/financials/components/LogPaymentForm.tsx`
```tsx
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { Button, Form as BootstrapForm } from 'react-bootstrap';

// This schema directly implements the business rules
const PaymentValidationSchema = Yup.object().shape({
  payerId: Yup.string().required('Payer is required.'),
  amount: Yup.number()
    .positive('Amount must be a positive number.')
    .required('Amount is required.'),
  date: Yup.date().required('Date is required.'),
  description: Yup.string().optional(),
});

export const LogPaymentForm = ({ onSubmit }) => {
  return (
    <Formik
      initialValues={{ payerId: '', amount: 0, date: new Date(), description: '' }}
      validationSchema={PaymentValidationSchema}
      onSubmit={onSubmit}
    >
      {() => (
        <Form>
          {/* Payer selection (e.g., a searchable dropdown) */}
          <BootstrapForm.Group className="mb-3">
            <label htmlFor="payerId">Payer</label>
            <Field name="payerId" as="select" className="form-select">
              <option value="">Select a player</option>
              {/* Populate options from an API call */}
            </Field>
            <ErrorMessage name="payerId" component="div" className="text-danger" />
          </BootstrapForm.Group>
          
          <BootstrapForm.Group className="mb-3">
            <label htmlFor="amount">Amount</label>
            <Field name="amount" type="number" className="form-control" />
            <ErrorMessage name="amount" component="div" className="text-danger" />
          </BootstrapForm.Group>

          {/* ... other fields for Date, Description ... */}

          <Button type="submit">Log Payment</Button>
        </Form>
      )}
    </Formik>
  );
};
```

## 5. Testing Strategy

The testing strategy will focus on unit tests for the critical validation logic on both the backend and frontend.

### 5.1. Backend Unit Test for Service Validation

**Action:** Create a unit test for `FinancialRecordService` to ensure validation rules are enforced.

**File:** `Diquis.Application.Tests/Services/FinancialRecordServiceTests.cs`
```csharp
using Diquis.Application.Services.FinancialRecords;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace Diquis.Application.Tests.Services;

[TestFixture]
public class FinancialRecordServiceTests
{
    private FinancialRecordService _service;
    private Mock<ApplicationDbContext> _mockContext;
    // ... setup for mocks

    [Test]
    public async Task CreateFinancialRecordAsync_WithNegativeAmount_ShouldThrowValidationException()
    {
        // Arrange
        var request = new CreateFinancialRecordRequest { Amount = -100, PayerId = Guid.NewGuid() };

        // Act
        Func<Task> act = async () => await _service.CreateFinancialRecordAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Payment amount must be a positive number.");
    }
    
    [Test]
    public async Task CreateFinancialRecordAsync_WithZeroAmount_ShouldThrowValidationException()
    {
        // Arrange
        var request = new CreateFinancialRecordRequest { Amount = 0, PayerId = Guid.NewGuid() };

        // Act
        Func<Task> act = async () => await _service.CreateFinancialRecordAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Payment amount must be a positive number.");
    }
}
```

### 5.2. Frontend Unit Test for Form Validation

**Action:** Create a fast unit test for the Yup validation schema to verify client-side rules.

**File:** `src/features/financials/components/LogPaymentForm.test.ts`
```ts
import { describe, it, expect } from 'vitest';
// Import the Yup schema from LogPaymentForm.tsx
import { PaymentValidationSchema } from './LogPaymentForm';

describe('PaymentValidationSchema', () => {
  it('should fail if amount is negative', async () => {
    const invalidPayment = { payerId: 'some-id', amount: -50, date: new Date() };
    await expect(PaymentValidationSchema.validate(invalidPayment)).rejects.toThrow('Amount must be a positive number.');
  });

  it('should fail if amount is zero', async () => {
    const invalidPayment = { payerId: 'some-id', amount: 0, date: new Date() };
    await expect(PaymentValidationSchema.validate(invalidPayment)).rejects.toThrow('Amount must be a positive number.');
  });

  it('should fail if payerId is missing', async () => {
    const invalidPayment = { payerId: '', amount: 50, date: new Date() };
    await expect(PaymentValidationSchema.validate(invalidPayment)).rejects.toThrow('Payer is required.');
  });

  it('should pass with valid data', async () => {
    const validPayment = { payerId: 'some-id', amount: 50, date: new Date() };
    await expect(PaymentValidationSchema.validate(validPayment)).resolves.toBe(validPayment);
  });
});
```
This test confirms that the client-side validation logic is correct, preventing invalid data from being sent to the API.
