# Clean Architecture Refactoring: Background Job Interfaces

## Overview
Moved background job interfaces from Infrastructure layer to Application layer to properly follow Clean Architecture principles.

## Changes Made

### ? Interface Moved to Application Layer

**Before:**
```
Diquis.Infrastructure/
  ??? BackgroundJobs/
      ??? IJobClientWrapper.cs  ? Wrong layer!
      ??? JobClientWrapper.cs
      ??? HangfireJobService.cs
```

**After:**
```
Diquis.Application/
  ??? Common/
      ??? IBackgroundJobService.cs
      ??? BackgroundJobs/
          ??? IJobClientWrapper.cs  ? Correct layer!

Diquis.Infrastructure/
  ??? BackgroundJobs/
      ??? JobClientWrapper.cs      ? Implements IJobClientWrapper
      ??? HangfireJobService.cs     ? Implements IBackgroundJobService
```

## Clean Architecture Layers

### Domain Layer (Pure Business Logic)
- ? NO infrastructure concerns
- ? NO application orchestration
- ? Only business entities, value objects, domain events

### Application Layer (Use Cases & Orchestration)
- ? **IBackgroundJobService** - Defines WHAT operations are available
- ? **IJobClientWrapper** - Abstracts HOW jobs are enqueued
- ? Use case implementations
- ? DTOs and interfaces for external concerns

### Infrastructure Layer (Implementation Details)
- ? **HangfireJobService** - Implements IBackgroundJobService using Hangfire
- ? **JobClientWrapper** - Wraps Hangfire's IBackgroundJobClient
- ? **TestJob** - Concrete job implementation
- ? Database contexts, external API clients, etc.

## Why This is Better

### 1. **Correct Dependency Direction**
```
Domain ? Application ? Infrastructure
   ?         ?              ?
   Pure    Abstracts    Implements
```

### 2. **Testability**
- Application layer can be tested with mock implementations
- No need to reference Hangfire in application tests
- Can swap Hangfire for Azure Functions, AWS Lambda, etc.

### 3. **Separation of Concerns**
- **Application**: "I need to schedule a job"
- **Infrastructure**: "I'll use Hangfire to do that"

### 4. **Consistency**
- Matches existing pattern with `IBackgroundJobService`
- All abstractions in Application layer
- All implementations in Infrastructure layer

## Files Updated

1. ? Created: `Diquis.Application\Common\BackgroundJobs\IJobClientWrapper.cs`
2. ? Updated: `Diquis.Infrastructure\BackgroundJobs\JobClientWrapper.cs`
3. ? Updated: `Diquis.Infrastructure\BackgroundJobs\HangfireJobService.cs`
4. ? Updated: `Diquis.WebApi\Program.cs`
5. ? Updated: `Diquis.BackgroundJobs\Extensions\ServiceCollectionExtensions.cs`
6. ? Removed: `Diquis.Infrastructure\BackgroundJobs\IJobClientWrapper.cs`

## Verification

Build the solution to ensure all references are updated:
```powershell
dotnet build .\Diquis.sln
```

All projects should build successfully with the new structure.

## Best Practices Applied

? **Interface Segregation Principle** - Small, focused interfaces  
? **Dependency Inversion Principle** - Depend on abstractions, not concretions  
? **Single Responsibility Principle** - Each layer has one reason to change  
? **Clean Architecture** - Dependencies point inward toward domain  

## Future Considerations

This structure makes it easy to:
- Add new job processing implementations (Azure Functions, MassTransit, etc.)
- Mock background jobs in integration tests
- Replace Hangfire without touching application layer
- Add multiple job processors simultaneously
