# Task Context
Define the `DocumentTemplate` and `SignedDocument` entities. Implement the `DigitalSignatureService` to handle PDF generation (using QuestPDF or similar), signature embedding, and "sealing" the document.

# Core References
- **Plan:** [18 - Implementation_Plan_DigitalSignatures.md](./18%20-%20Implementation_Plan_DigitalSignatures.md)
- **Tech Guide:** [UtilityModules_TechnicalGuide.md](../../Technical%20documentation/UtilityModules_TechnicalGuide/UtilityModules_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `DocumentTemplate.cs`:**
    *   Path: `Diquis.Domain/Entities/DocumentTemplate.cs`.
    *   Properties: `Title`, `Content` (HTML/Markdown), `DocumentType` (Enum), `TenantId`.
2.  **Create `SignedDocument.cs`:**
    *   Path: `Diquis.Domain/Entities/SignedDocument.cs`.
    *   Properties: `DocumentTemplateId`, `PlayerId`, `SignatoryId`, `SealedPdfUrl`, `SignatureAuditTrail` (JSON), `ExpiresAt`, `TenantId`.
3.  **Implement `DigitalSignatureService.cs`:**
    *   Path: `Diquis.Application/Services/Signatures/DigitalSignatureService.cs`.
    *   `GenerateAndSendSignatureRequestAsync`: Create PDF from template, send email with secure link.
    *   `FinalizeSignatureAsync`: Validate token, embed signature image into PDF, upload to storage, create `SignedDocument`, update `ApplicationUser.WaiverStatus` to `Current`.
4.  **Unit Test:**
    *   Create `Diquis.Infrastructure.Tests/Signatures/SignaturePersistenceTests.cs`.
    *   Verify persistency and status update.

# Acceptance Criteria
- [ ] Entities created.
- [ ] `DigitalSignatureService` implements PDF generation and sealing.
- [ ] Unit tests pass.
