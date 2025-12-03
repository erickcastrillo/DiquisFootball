# Sports Medicine: Implementation & Testing Plan

## 1. Executive Summary

This document details the implementation and testing strategy for the Sports Medicine module. This is a high-security module responsible for managing confidential player injury records and rehabilitation progress. The architecture prioritizes data privacy through field-level encryption and strict, role-based access policies.

The plan outlines the domain model for injury records, the backend implementation of encryption and authorization, a frontend design that visually communicates data security, and a testing strategy focused on verifying the robust access control mechanisms.

## 2. Architectural Blueprint: Domain Entities & Encryption

The domain model is designed to capture the injury lifecycle while ensuring sensitive data is marked for encryption at the persistence layer.

**Action:** Create the `InjuryRecord` entity in the `Diquis.Domain` project.

**File:** `Diquis.Domain/Entities/InjuryRecord.cs`
```csharp
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;

namespace Diquis.Domain.Entities;

/// <summary>
/// Represents a confidential medical record for a player's injury.
/// Marked as [Auditable] to create a log of all changes.
/// </summary>
[Auditable]
public class InjuryRecord : BaseEntity, IMustHaveTenant
{
    public Guid PlayerId { get; set; }
    public ApplicationUser Player { get; set; }

    public DateOnly DateOfInjury { get; set; }
    public InjuryStatus InjuryStatus { get; set; } = InjuryStatus.Active; // Enum: Active, Closed
    public RtpStatus RtpStatus { get; set; } = RtpStatus.NotCleared; // Enum: NotCleared, ClearedForNonContact, etc.

    // --- ENCRYPTED FIELDS ---
    // These fields will be encrypted at the database level by the DbContext.
    public string BodyPart { get; set; }
    public string InjuryType { get; set; }
    public string Diagnosis { get; set; }
    public string? MechanismOfInjury { get; set; }
    // ------------------------

    public ICollection<RehabilitationLog> RehabilitationLogs { get; set; } = new List<RehabilitationLog>();
    public required string TenantId { get; set; }
}
```
**Encryption Requirement:** All string properties marked as sensitive (`BodyPart`, `Diagnosis`, etc.) **must** be configured in the `DbContext` to use a value converter that provides application-level encryption before the data is persisted.

## 3. Backend Implementation: Encryption & Authorization

The backend implementation focuses on two critical aspects: transparently encrypting sensitive data and enforcing granular access policies.

### 3.1. Field-Level Encryption

As specified in the technical guide, we will use an EF Core `ValueConverter` combined with an encryption service to protect data at rest.

**Action:** Implement and apply the `EncryptedStringConverter` in `ApplicationDbContext`.

**File:** `Diquis.Infrastructure/Persistence/Contexts/ApplicationDbContext.cs` (enhancement)
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Key MUST be loaded securely from configuration (e.g., Azure Key Vault).
    var encryptionKey = _config["Security:DbEncryptionKey"];
    if (string.IsNullOrEmpty(encryptionKey)) 
        throw new InvalidOperationException("DB Encryption Key is not configured.");

    var encryptionService = new AesEncryptionService(encryptionKey);
    var encryptedStringConverter = new EncryptedStringConverter(encryptionService);

    // Apply encryption to all sensitive fields in the InjuryRecord entity
    var injuryEntity = modelBuilder.Entity<InjuryRecord>();
    injuryEntity.Property(p => p.BodyPart).HasConversion(encryptedStringConverter);
    injuryEntity.Property(p => p.InjuryType).HasConversion(encryptedStringConverter);
    injuryEntity.Property(p => p.Diagnosis).HasConversion(encryptedStringConverter);
    injuryEntity.Property(p => p.MechanismOfInjury).HasConversion(encryptedStringConverter);
}
```

### 3.2. "Medical Staff Only" Authorization Policies

Access to medical data will be protected by a dedicated authorization policy.

**Action:** Define and register the `IsMedicalStaff` policy.

**File:** `Diquis.Infrastructure/Auth/PolicyServiceCollectionExtensions.cs` (enhancement)
```csharp
public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
{
    services.AddAuthorization(options =>
    {
        // ... other policies
        options.AddPolicy("IsMedicalStaff", policy => 
            policy.RequireClaim("permission", "medical.fullaccess"));
    });
    return services;
}
```
This policy will then be applied to all CUD (Create, Update, Delete) endpoints in the `InjuryRecordsController`. Read access is handled dynamically within the service layer.

## 4. Frontend Implementation (React)

The frontend will provide a secure interface for managing medical records, visually indicating to users which data is encrypted.

### 4.1. Folder Structure

**Action:** Create the new feature folder `src/features/medical`.

### 4.2. Secure Notes & Encrypted Field Indicators

When medical staff are entering data, the UI should provide visual cues that the information is being secured.

**Action:** Implement the `InjuryForm` with visual indicators.

**File:** `src/features/medical/components/InjuryForm.tsx`
```tsx
import { Formik, Form, Field } from 'formik';
import { Button, Form as BootstrapForm, InputGroup } from 'react-bootstrap';

// A small component for the lock icon
const SecureFieldAddon = () => (
  <InputGroup.Text>
    <i className="bi bi-lock-fill" title="This field is encrypted"></i>
  </InputGroup.Text>
);

export const InjuryForm = ({ onSubmit }) => {
  return (
    <Formik initialValues={{ diagnosis: '', bodyPart: '' }} onSubmit={onSubmit}>
      {() => (
        <Form>
          <BootstrapForm.Group className="mb-3">
            <label htmlFor="bodyPart">Body Part</label>
            <InputGroup>
              <Field name="bodyPart" className="form-control" />
              <SecureFieldAddon />
            </InputGroup>
          </BootstrapForm.Group>
        
          <BootstrapForm.Group className="mb-3">
            <label htmlFor="diagnosis">Diagnosis</label>
            <InputGroup>
              <Field name="diagnosis" as="textarea" rows={4} className="form-control" />
              <SecureFieldAddon />
            </InputGroup>
          </BootstrapForm.Group>

          <Button type="submit">Save Injury Record</Button>
        </Form>
      )}
    </Formik>
  );
};
```
This UI pattern provides immediate feedback to the user, building trust and reinforcing the security of the system.

## 5. Testing Strategy

The highest priority is to test the authorization boundaries, ensuring that non-medical staff absolutely cannot access sensitive medical details.

### 5.1. Backend Security Integration Test

This test validates that the API endpoints are correctly secured.

**Action:** Create a security-focused integration test for the `InjuryRecordsController`.

**File:** `Diquis.WebApi.Tests/Security/MedicalApiSecurityTests.cs`
```csharp
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Xunit;

[TestFixture]
public class MedicalApiSecurityTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MedicalApiSecurityTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Test]
    public async Task GetInjuryDetails_WithMedicalStaffToken_ShouldReturnOk()
    {
        // Arrange
        var token = await TestAuthHelper.GetTokenForUserWithRoleAsync("MedicalStaff");
        var injuryId = TestData.SampleInjuryId; // Assume a seeded injury record
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/injury-records/{injuryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetInjuryDetails_WithCoachToken_ShouldReturnForbidden()
    {
        // Arrange
        // This test assumes the fine-grained check is in the service layer,
        // and the coach is not the player/parent of the player with the injury.
        var token = await TestAuthHelper.GetTokenForUserWithRoleAsync("Coach");
        var injuryId = TestData.SampleInjuryId;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Act
        var response = await _client.GetAsync($"/api/injury-records/{injuryId}");

        // Assert
        // A 403 Forbidden response proves the user was authenticated but not authorized.
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task CreateInjuryRecord_WithCoachToken_ShouldReturnForbidden()
    {
        // Arrange
        var token = await TestAuthHelper.GetTokenForUserWithRoleAsync("Coach");
        var createRequest = new CreateInjuryRecordRequest { /* ...data... */ };
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/injury-records", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
```
This test suite provides high confidence that the API layer correctly enforces the "Medical Staff Only" policy for both read and write operations, fulfilling the core security requirement of the module.
