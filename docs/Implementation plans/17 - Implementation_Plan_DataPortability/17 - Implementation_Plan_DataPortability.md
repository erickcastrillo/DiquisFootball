# Data Portability: Implementation & Testing Plan

## 1. Executive Summary

This document provides a detailed implementation and testing plan for the Data Portability module. This module is critical for seamless academy onboarding, allowing users to bulk-import existing data via a "Smart Import" wizard. It focuses on background job processing, robust data validation, and a clear user experience for managing the import process.

The plan outlines the architectural blueprint for tracking import jobs, the backend validation logic, the frontend implementation of a multi-step import wizard, and a testing strategy focused on ensuring invalid data is handled gracefully.

## 2. Architectural Blueprint: The `ImportJob` Entity

While the technical guide correctly notes that this module does not add new core domain entities like "Player," a tracking entity is essential for managing the state of asynchronous import operations. We will introduce an `ImportJob` entity for this purpose.

**Action:** Create the `ImportJob.cs` entity file in the `Diquis.Domain` project.

**File:** `Diquis.Domain/Entities/ImportJob.cs`
```csharp
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;

namespace Diquis.Domain.Entities;

/// <summary>
/// Represents and tracks the status of a single data import process.
/// </summary>
public class ImportJob : BaseEntity, IMustHaveTenant
{
    public required string OriginalFilename { get; set; }
    public ImportJobStatus Status { get; set; } = ImportJobStatus.Pending;
    public int TotalRowCount { get; set; }
    public int ProcessedRowCount { get; set; }
    
    /// <summary>
    /// A link to a downloadable report (e.g., CSV/text file in blob storage)
    /// detailing the validation errors, if any.
    /// </summary>
    public string? ErrorReportUrl { get; set; }

    public required string TenantId { get; set; }
}

public enum ImportJobStatus
{
    Pending,
    Uploading,
    Validating,
    FailedValidation,
    Importing,
    Completed,
    Failed
}
```

## 3. Backend Implementation: Invalid Data Validation

The core of the backend implementation is the background job that processes the uploaded CSV file. A critical step in this job is the pre-import validation run, which checks every row for errors before any data is committed to the database.

**Action:** Implement the validation logic within the `DataPortabilityJob`.

**File:** `Diquis.Infrastructure/BackgroundJobs/DataPortabilityJob.cs` (enhancement)
```csharp
using CsvHelper;
using System.Globalization;

public class DataPortabilityJob : IDataPortabilityJob
{
    private readonly ApplicationDbContext _context;
    // ... other services

    public async Task ProcessPlayerImport(Guid importJobId, string tempFilePath)
    {
        var importJob = await _context.ImportJobs.FindAsync(importJobId);
        if (importJob is null) return;

        // --- Step 1: Validation Dry Run ---
        importJob.Status = ImportJobStatus.Validating;
        await _context.SaveChangesAsync();

        var errors = new List<string>();
        var records = new List<PlayerImportDto>();
        
        using (var reader = new StreamReader(tempFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = csv.GetRecords<PlayerImportDto>().ToList();
        }

        for (int i = 0; i < records.Count; i++)
        {
            var record = records[i];
            if (string.IsNullOrWhiteSpace(record.FullName))
                errors.Add($"Row {i + 2}: 'FullName' is a required field and is missing.");
            if (record.DateOfBirth is null)
                errors.Add($"Row {i + 2}: 'DateOfBirth' is a required field and is missing.");
            // ... more validation for email format, data types, etc. ...
        }

        // --- Step 2: Handle Validation Outcome ---
        if (errors.Any())
        {
            importJob.Status = ImportJobStatus.FailedValidation;
            // Create a report file from the errors list
            var errorFileUrl = await _fileStorageService.UploadErrorReportAsync(errors);
            importJob.ErrorReportUrl = errorFileUrl;
            await _context.SaveChangesAsync();
            return; // Stop processing
        }

        // --- Step 3: If validation passes, proceed to import ---
        importJob.Status = ImportJobStatus.Importing;
        await _context.SaveChangesAsync();
        // ... Use a bulk-insert library to efficiently create player records ...
        // ... Update status to Completed ...
    }
}
```

## 4. Frontend Implementation (React)

A new feature folder will house the "Smart Import" wizard.

### 4.1. Folder Structure

**Action:** Create the new feature folder `src/features/data-management`.

### 4.2. Multi-Step Import Wizard Modal

The import process will be guided by a multi-step modal that walks the user through uploading, mapping (future step), and monitoring the import.

**Action:** Create the `ImportWizardModal` component.

**File:** `src/features/data-management/components/ImportWizardModal.tsx`
```tsx
import { useState, useEffect } from 'react';
import { Modal, Button, ProgressBar, Alert } from 'react-bootstrap';
import { useDataManagementApi } from '../hooks/useDataManagementApi';

const POLLING_INTERVAL = 5000; // 5 seconds

export const ImportWizardModal = ({ show, onHide }) => {
  const [file, setFile] = useState<File | null>(null);
  const [jobId, setJobId] = useState<string | null>(null);
  const [jobStatus, setJobStatus] = useState<any>(null);
  const { uploadFile, getJobStatus } = useDataManagementApi();

  // Polling effect to check job status
  useEffect(() => {
    if (!jobId) return;

    const interval = setInterval(() => {
      getJobStatus(jobId).then(status => {
        setJobStatus(status);
        if (['Completed', 'FailedValidation', 'Failed'].includes(status.status)) {
          clearInterval(interval);
        }
      });
    }, POLLING_INTERVAL);

    return () => clearInterval(interval);
  }, [jobId, getJobStatus]);

  const handleUpload = async () => {
    if (!file) return;
    const { id } = await uploadFile(file); // API call to start the job
    setJobId(id);
  };

  const renderStep = () => {
    if (jobId && jobStatus) {
      // Step 2: Progress Monitoring
      return (
        <div>
          <h4>Import in Progress...</h4>
          <p>Status: <strong>{jobStatus.status}</strong></p>
          <ProgressBar now={(jobStatus.processedRowCount / jobStatus.totalRowCount) * 100} label={`${jobStatus.processedRowCount} / ${jobStatus.totalRowCount}`} />
          {jobStatus.status === 'FailedValidation' && (
            <Alert variant="danger" className="mt-3">
              Validation failed. Please correct the errors in your file.
              <a href={jobStatus.errorReportUrl} className="btn btn-link">Download Error Report</a>
            </Alert>
          )}
          {jobStatus.status === 'Completed' && <Alert variant="success" className="mt-3">Import completed successfully!</Alert>}
        </div>
      );
    }
    
    // Step 1: File Upload
    return (
      <div>
        <h4>Upload Player Data</h4>
        <p>Download the <a href="/templates/player-import-template.csv">template CSV file</a> to get started.</p>
        <input type="file" className="form-control" onChange={e => setFile(e.target.files?.[0] || null)} />
      </div>
    );
  };

  return (
    <Modal show={show} onHide={onHide}>
      <Modal.Header closeButton>
        <Modal.Title>Smart Import Wizard</Modal.Title>
      </Modal.Header>
      <Modal.Body>{renderStep()}</Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onHide}>Close</Button>
        {!jobId && <Button variant="primary" onClick={handleUpload} disabled={!file}>Start Import</Button>}
      </Modal.Footer>
    </Modal>
  );
};
```

## 5. Testing Strategy

The test strategy will focus on ensuring the backend job correctly identifies invalid data and halts the import process, providing a useful error report.

### 5.1. Backend Integration Test: Invalid CSV Handling

This test validates that the `DataPortabilityJob` performs its "dry run" validation correctly and updates the job status accordingly without committing any data.

**Action:** Create an integration test for the `DataPortabilityJob`.

**File:** `Diquis.Infrastructure.Tests/DataPortability/ImportValidationTests.cs`
```csharp
using FluentAssertions;
using NUnit.Framework;

[TestFixture]
public class ImportValidationTests
{
    private DataPortabilityJob _job;
    private ApplicationDbContext _context;
    private Mock<IFileStorageService> _mockFileStorage;

    [SetUp]
    public async Task Setup()
    {
        _context = /* ... get in-memory context ... */;
        _mockFileStorage = new Mock<IFileStorageService>();
        _job = new DataPortabilityJob(_context, _mockFileStorage.Object);
    }

    [Test]
    public async Task ProcessPlayerImport_WithInvalidCsv_ShouldFailValidationAndGenerateReport()
    {
        // ARRANGE
        // 1. Create a mock CSV file with errors
        var csvContent = "FullName,DateOfBirth\n,2010-01-01\nJohn Doe,"; // Row 2 missing name, Row 3 missing DOB
        var tempFilePath = TestHelper.CreateTempFile(csvContent);

        // 2. Create the tracking ImportJob entity
        var importJob = new ImportJob { OriginalFilename = "test.csv" };
        _context.ImportJobs.Add(importJob);
        await _context.SaveChangesAsync();
        
        // 3. Mock the error report upload
        _mockFileStorage.Setup(s => s.UploadErrorReportAsync(It.IsAny<List<string>>()))
                        .ReturnsAsync("http://storage.com/error-report.txt");

        // ACT
        await _job.ProcessPlayerImport(importJob.Id, tempFilePath);

        // ASSERT
        // 1. Verify no players were created in the database
        var playerCount = await _context.Users.CountAsync(u => u.UserRoles.Any(ur => ur.Role.Name == "Player"));
        playerCount.Should().Be(0);

        // 2. Verify the ImportJob status is updated correctly
        var updatedJob = await _context.ImportJobs.FindAsync(importJob.Id);
        updatedJob.Status.Should().Be(ImportJobStatus.FailedValidation);
        updatedJob.ErrorReportUrl.Should().Be("http://storage.com/error-report.txt");
        
        // 3. Verify the error report was generated
        _mockFileStorage.Verify(s => s.UploadErrorReportAsync(
            It.Is<List<string>>(errors => 
                errors.Count == 2 &&
                errors.Contains("Row 2: 'FullName' is a required field and is missing.")
            )), Times.Once);
    }
}
```
This test confirms that the system correctly validates data, prevents bad data from entering the database, and provides clear feedback to the user, fulfilling the core requirements of the "Smart Import" feature.
