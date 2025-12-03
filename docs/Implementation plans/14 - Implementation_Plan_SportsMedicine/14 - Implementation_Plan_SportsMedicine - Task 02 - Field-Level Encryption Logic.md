# Task Context
Implement field-level encryption for sensitive medical data. This involves creating an encryption service and a Value Converter for EF Core to transparently encrypt/decrypt data.

# Core References
- **Plan:** [14 - Implementation_Plan_SportsMedicine.md](./14%20-%20Implementation_Plan_SportsMedicine.md)

# Step-by-Step Instructions
1.  **Create `AesEncryptionService`:**
    *   Implement `IEncryptionService` (Encrypt/Decrypt methods).
    *   Use `System.Security.Cryptography.Aes`.
    *   Load key from configuration.
2.  **Create `EncryptedStringConverter`:**
    *   Inherit from `ValueConverter<string, string>`.
    *   Use `AesEncryptionService` in the conversion expressions.
3.  **Configure `ApplicationDbContext`:**
    *   In `OnModelCreating`, apply `EncryptedStringConverter` to `InjuryRecord` properties: `BodyPart`, `InjuryType`, `Diagnosis`, `MechanismOfInjury`.
4.  **Register Policy:**
    *   Add `IsMedicalStaff` policy (`permission:medical.fullaccess`) in `PolicyServiceCollectionExtensions.cs`.

# Acceptance Criteria
- [ ] `AesEncryptionService` and `EncryptedStringConverter` are implemented.
- [ ] `ApplicationDbContext` applies encryption to sensitive fields.
- [ ] `IsMedicalStaff` policy is registered.
