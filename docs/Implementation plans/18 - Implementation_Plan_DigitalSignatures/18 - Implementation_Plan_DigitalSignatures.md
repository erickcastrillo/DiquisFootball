# Digital Signatures: Implementation & Testing Plan

## 1. Executive Summary

This document outlines the implementation and testing strategy for the Digital Signatures module. This system provides a legally compliant and user-friendly workflow for signing waivers and consent forms, which is critical for academy operations and risk management. It includes a "blocker" logic that prevents players from participating in team activities until required documents are signed.

The plan details the domain model for document templates and the resulting signed documents, the frontend implementation of a canvas-based signature pad, and a testing strategy to validate the end-to-end persistence of a captured signature.

## 2. Architectural Blueprint: Signature & Document Entities

The architecture relies on two primary entities: a `DocumentTemplate` that administrators create, and a `SignedDocument` which is the immutable record of a signature event.

**Action:** Create the following entity files in the `Diquis.Domain/Entities/` directory.

**File:** `Diquis.Domain/Entities/DocumentTemplate.cs`
```csharp
namespace Diquis.Domain.Entities;

/// <summary>
/// An admin-managed template for a document that requires a signature.
/// Supports placeholders like {{PlayerName}} and {{ParentName}}.
/// </summary>
public class DocumentTemplate : BaseEntity, IMustHaveTenant
{
    public required string Title { get; set; }
    public required string Content { get; set; } // HTML or Markdown content
    public DocumentType DocumentType { get; set; } // Enum: Waiver, Consent, Contract
    public required string TenantId { get; set; }
}
```

**File:** `Diquis.Domain/Entities/SignedDocument.cs` (Represents the "Signature")
```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace Diquis.Domain.Entities;

/// <summary>
/// An immutable record of a completed signature event for a specific document and player.
/// </summary>
public class SignedDocument : BaseEntity, IMustHaveTenant
{
    public Guid DocumentTemplateId { get; set; }
    public DocumentTemplate DocumentTemplate { get; set; }

    public Guid PlayerId { get; set; }
    public ApplicationUser Player { get; set; }

    public Guid SignatoryId { get; set; } // The User who signed (parent or player)
    public ApplicationUser Signatory { get; set; }
    
    /// <summary>
    /// URL to the final, sealed PDF stored in secure blob storage.
    /// </summary>
    public required string SealedPdfUrl { get; set; }

    /// <summary>
    /// A JSON blob containing legally-compliant audit trail info
    /// (IP Address, Timestamp, User Agent).
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required string SignatureAuditTrail { get; set; }

    public DateTime? ExpiresAt { get; set; }
    public required string TenantId { get; set; }
}
```

## 3. Backend Implementation: Blocker Logic & Signature Finalization

A crucial part of the backend is enforcing the "blocker logic" specified in the FRS. This involves modifying existing services to check a player's `WaiverStatus` before allowing certain actions. The `DigitalSignatureService` will handle the finalization of the signature itself.

**Action:** Modify existing services to check for a valid waiver status.

**File:** `Diquis.Application/Services/Teams/TeamService.cs` (enhancement)
```csharp
public async Task UpdateRosterAsync(Guid teamId, UpdateRosterRequest request)
{
    // ... authorization checks ...

    // CRUCIAL: Blocker Logic Integration
    var playersToAdd = await _context.Users
        .Where(u => request.PlayerIdsToAdd.Contains(u.Id))
        .ToListAsync();

    var un-waivedPlayer = playersToAdd.FirstOrDefault(p => p.WaiverStatus != WaiverStatus.Current);
    if (unwaivedPlayer != null)
    {
        throw new ValidationException($"Cannot add '{unwaivedPlayer.FullName}': Missing signed waiver.");
    }
    
    // ... proceed with transaction to update roster ...
}
```
The `DigitalSignatureService` will contain the `FinalizeSignatureAsync` method, which takes the Base64 signature data, embeds it into a PDF, uploads the sealed PDF to storage, creates the `SignedDocument` record, and updates the player's `WaiverStatus`.

## 4. Frontend Implementation (React)

A new feature folder will contain the UI for capturing and submitting a digital signature.

### 4.1. Folder Structure & Setup

**Action:** Create the folder `src/features/signatures` and install a signature pad library.
```bash
npm install react-signature-canvas
```

### 4.2. Signature Pad Component

This component will render the canvas for capturing the user's signature.

**Action:** Create the `SignaturePad` component.

**File:** `src/features/signatures/components/SignaturePad.tsx`
```tsx
import { useRef } from 'react';
import SignatureCanvas from 'react-signature-canvas';
import { Button, Card } from 'react-bootstrap';

interface SignaturePadProps {
  onSign: (signature: string) => void; // Callback with Base64 signature data
}

export const SignaturePad = ({ onSign }: SignaturePadProps) => {
  const sigPadRef = useRef<SignatureCanvas>(null);

  const clear = () => {
    sigPadRef.current?.clear();
  };

  const submitSignature = () => {
    if (sigPadRef.current?.isEmpty()) {
      alert('Please provide a signature first.');
      return;
    }
    // Get the signature as a Base64 encoded PNG
    const signatureDataUrl = sigPadRef.current?.toDataURL();
    onSign(signatureDataUrl);
  };

  return (
    <Card>
      <Card.Header>Please Sign Below</Card.Header>
      <Card.Body>
        <div style={{ border: '1px solid #ccc' }}>
          <SignatureCanvas
            ref={sigPadRef}
            penColor="black"
            canvasProps={{ width: 500, height: 200, className: 'sigCanvas' }}
          />
        </div>
      </Card.Body>
      <Card.Footer>
        <Button variant="secondary" onClick={clear}>Clear</Button>
        <Button variant="primary" onClick={submitSignature} className="ms-2">
          Confirm and Sign Document
        </Button>
      </Card.Footer>
    </Card>
  );
};
```
This component would be displayed on a secure page (`/sign/{jwt}`) that shows the document to be signed and includes this signature pad at the bottom.

## 5. Testing Strategy

The testing strategy must verify that a captured signature is correctly processed and that all required database records are created or updated in a single, atomic transaction.

### 5.1. Backend Integration Test: Signature Persistency

This test validates the `FinalizeSignatureAsync` workflow, ensuring that submitting a signature correctly creates a `SignedDocument` and updates the player's status.

**Action:** Create an integration test for the `DigitalSignatureService`.

**File:** `Diquis.Infrastructure.Tests/Signatures/SignaturePersistenceTests.cs`
```csharp
using FluentAssertions;
using Moq;
using NUnit.Framework;

[TestFixture]
public class SignaturePersistenceTests
{
    private DigitalSignatureService _service;
    private ApplicationDbContext _context;
    private Mock<IFileStorageService> _mockFileStorage; // To mock PDF upload

    [SetUp]
    public async Task Setup()
    {
        _context = /* ... get in-memory context ... */;
        _mockFileStorage = new Mock<IFileStorageService>();
        _service = new DigitalSignatureService(_context, _mockFileStorage.Object, /* other mocks */);

        // ARRANGE: Seed DB with a player requiring a signature
        var player = new ApplicationUser { Id = Guid.NewGuid(), WaiverStatus = WaiverStatus.Pending };
        _context.Users.Add(player);
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task FinalizeSignatureAsync_WithValidSignature_ShouldPersistDocumentAndUpdatePlayerStatus()
    {
        // ARRANGE
        var player = await _context.Users.FirstAsync();
        var finalizeRequest = new FinalizeSignatureRequest 
        { 
            PlayerId = player.Id,
            SignatureDataUrl = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA..." // Sample Base64 string
        };

        var sealedPdfUrl = "https://storage.com/sealed-doc.pdf";
        _mockFileStorage.Setup(s => s.UploadSealedDocumentAsync(It.IsAny<byte[]>())).ReturnsAsync(sealedPdfUrl);

        // ACT
        await _service.FinalizeSignatureAsync(finalizeRequest);

        // ASSERT
        // 1. Verify a SignedDocument record was created
        var signedDoc = await _context.SignedDocuments.FirstOrDefaultAsync(d => d.PlayerId == player.Id);
        signedDoc.Should().NotBeNull();
        signedDoc.SealedPdfUrl.Should().Be(sealedPdfUrl);
        signedDoc.SignatureAuditTrail.Should().NotBeNullOrEmpty();

        // 2. Verify the player's status was updated to 'Current'
        var updatedPlayer = await _context.Users.FindAsync(player.Id);
        updatedPlayer.WaiverStatus.Should().Be(WaiverStatus.Current);
        
        // 3. Verify that the file was "uploaded"
        _mockFileStorage.Verify(s => s.UploadSealedDocumentAsync(It.IsAny<byte[]>()), Times.Once);
    }
}
```
This test provides high confidence that the end-to-end process of finalizing a signature is working correctly, from persisting the legal record to unblocking the player for academy activities.
