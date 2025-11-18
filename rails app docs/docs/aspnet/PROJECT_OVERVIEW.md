# Diquis - ASP.NET Core Project Overview

**Project Name:** Diquis Football Management System  
**Framework:** ASP.NET Core 10.0+  
**Architecture:** Clean Architecture with Vertical Slices  
**Database:** PostgreSQL or SQL Server with GUID primary keys  
**Created:** October 13, 2025

## Project Description

Diquis is a comprehensive football (soccer) academy management system designed to manage players, teams, training sessions, and administrative tasks across multiple academies. The system implements multi-tenant architecture where each academy operates as an isolated tenant with its own data and resources.

## Technology Stack

### Backend

- **Framework:** ASP.NET Core 10.0+ (Web API)
- **Language:** C# 12
- **Database:** PostgreSQL 15+ or SQL Server 2022+
- **ORM:** Entity Framework Core 10.0+
- **API Endpoints:** FastEndpoints (REPR pattern - Request-Endpoint-Response)
- **Multi-Tenancy:** Custom middleware with query filters
- **Authentication:** ASP.NET Core Identity + JWT Bearer tokens
- **Authorization:** Policy-based authorization with custom requirements
- **Background Jobs:** Hangfire or Quartz.NET (see comparison below)
- **Caching:** Redis (StackExchange.Redis)
- **File Storage:** Azure Blob Storage or AWS S3 (via SDK)
- **Image Processing:** ImageSharp
- **API Documentation:** FastEndpoints built-in Swagger/OpenAPI 3.0

### Frontend (Separate Repository)

- **Framework:** React
- **State Management:** Redux Toolkit
- **Data Fetching:** TanStack Query (React Query)
- **Build Tool:** Vite
- **Styling:** Tailwind CSS

### Development Tools

- **Build System:** MSBuild / dotnet CLI
- **Testing:** xUnit with Moq and FluentAssertions
- **Code Quality:** StyleCop.Analyzers, SonarAnalyzer.CSharp
- **Performance:** MiniProfiler, Application Insights
- **Hot Reload:** dotnet watch for development

## Architecture Principles

### 1. FastEndpoints Architecture (REPR Pattern)

The application uses **FastEndpoints** which implements the REPR pattern (Request-Endpoint-Response):

- **Endpoint Classes:** Each endpoint is a single class handling one operation
- **Request DTOs:** Strongly-typed request validation
- **Response DTOs:** Consistent response types
- **No Controllers:** Endpoints replace traditional controllers
- **Performance:** Faster than traditional ASP.NET Core controllers
- **Source Generators:** Compile-time code generation for better performance

### 2. Vertical Slice Organization

Features are organized by business capability (similar to Rails slices):

- Each feature contains related endpoints, validators, mappers
- **AcademyManagement:** Academy CRUD, user associations
- **PlayerManagement:** Player registration, search, updates
- **TeamManagement:** Team creation, rosters
- **TrainingManagement:** Scheduling, attendance tracking
- Minimal coupling between features

### 3. Service Layer Pattern

Business logic is handled through service classes:

- **Domain Services:** Complex business logic and rules
- **Application Services:** Orchestration and workflow
- **Infrastructure Services:** External integrations (email, storage)
- Services are injected into endpoints via dependency injection

### 4. Multi-Tenant Design

- Academy-based data isolation using custom middleware
- Global query filters in EF Core for automatic tenant scoping
- Tenant context from URL, headers, or user claims
- Shared resources (categories, divisions) across tenants

### 5. API-First Design

- RESTful JSON API using FastEndpoints
- Automatic request validation with FluentValidation
- Consistent response patterns with typed responses
- Built-in error handling with ProblemDetails
- OpenAPI/Swagger documentation auto-generated
- API versioning built into FastEndpoints

### 6. Domain-Driven Design

- Rich domain models with behavior
- Value objects for complex types
- Domain events for cross-aggregate communication
- Ubiquitous language throughout codebase

## Background Jobs: Hangfire vs Quartz.NET

Diquis can use either **Hangfire** or **Quartz.NET** for background job processing. Here's a comparison to help you choose:

### Hangfire

**Pros:**

- ✅ Built-in web dashboard for monitoring jobs
- ✅ Simpler API and easier to get started
- ✅ Automatic retries with exponential backoff
- ✅ Persistent storage (SQL Server, PostgreSQL, Redis)
- ✅ Fire-and-forget, delayed, and recurring jobs
- ✅ Background job continuations (chaining)
- ✅ Great for .NET-centric workflows

**Cons:**

- ❌ Commercial license required for some features in production
- ❌ Less flexible scheduling than Quartz
- ❌ Heavier resource usage

**Best For:** Applications that need a simple, batteries-included solution with excellent monitoring

### Quartz.NET

**Pros:**

- ✅ Completely free and open source (Apache License)
- ✅ Very mature and battle-tested (port of Java Quartz)
- ✅ Extremely flexible cron-based scheduling
- ✅ Clustered/distributed job execution
- ✅ Job persistence to database
- ✅ Misfire handling and recovery
- ✅ Lower resource footprint

**Cons:**

- ❌ No built-in web UI (requires third-party or custom solution)
- ❌ Steeper learning curve
- ❌ More verbose configuration
- ❌ Requires more setup for monitoring

**Best For:** Enterprise applications needing complex scheduling, clustering, or avoiding licensing costs

### Recommendation for Diquis

**For Development/Small Deployments:** Use **Hangfire**

- Dashboard is invaluable for debugging
- Simpler to set up and maintain
- Free tier sufficient for small academies

**For Production/Large Scale:** Use **Quartz.NET**

- No licensing costs at scale
- Better for distributed systems
- More control over scheduling

### Example: Same Job in Both

**Hangfire:**

```csharp
// Schedule job
BackgroundJob.Schedule<TrainingReminderJob>(
    job => job.SendRemindersAsync(trainingId),
    TimeSpan.FromHours(24));

// Recurring job
RecurringJob.AddOrUpdate(
    "send-daily-reports",
    () => GenerateDailyReportsAsync(),
    Cron.Daily);
```

**Quartz.NET:**

```csharp
// Schedule job
var job = JobBuilder.Create<TrainingReminderJob>()
    .WithIdentity("training-reminder", "notifications")
    .UsingJobData("trainingId", trainingId)
    .Build();

var trigger = TriggerBuilder.Create()
    .WithIdentity("training-reminder-trigger")
    .StartAt(DateTimeOffset.Now.AddHours(24))
    .Build();

await scheduler.ScheduleJob(job, trigger);

// Recurring job (cron)
var cronTrigger = TriggerBuilder.Create()
    .WithCronSchedule("0 0 0 * * ?") // Daily at midnight
    .Build();
```

Both options are well-supported in the .NET ecosystem. This documentation will show examples for both, and you can choose based on your requirements.

## Core Business Domains (Features)

### 1. Academy Management

- Academy creation and configuration
- User-academy associations
- Academy settings and preferences
- Owner/administrator management

**Key Entities:** `Academy`, `AcademyUser`  
**Key Endpoints:** `CreateAcademyEndpoint`, `UpdateAcademyEndpoint`, `GetAcademiesEndpoint`

### 2. Player Management

- Player registration and profiles
- Player search and filtering
- Player-position assignments
- Parent/guardian information
- Player skill assessments

**Key Entities:** `Player`, `PlayerSkill`  
**Key Endpoints:** `RegisterPlayerEndpoint`, `SearchPlayersEndpoint`, `GetPlayerEndpoint`

### 3. Team Management

- Team creation and organization
- Team rosters and memberships
- Team schedules
- Category/division assignments

**Key Entities:** `Team`, `TeamMembership`  
**Key Endpoints:** `CreateTeamEndpoint`, `AddPlayerToTeamEndpoint`, `GetTeamRosterEndpoint`

### 4. Training Management

- Training session scheduling
- Attendance tracking (bulk operations)
- Training types and locations
- Coach assignments
- Real-time updates (SignalR)

**Key Entities:** `Training`, `TrainingAttendance`  
**Key Endpoints:** `ScheduleTrainingEndpoint`, `RecordAttendanceEndpoint`, `GetTrainingsEndpoint`  
**Key Jobs:** `SendTrainingReminderJob`

### 5. Shared Resources

- Positions (Goalkeeper, Defender, etc.)
- Skills (Passing, Shooting, etc.)
- Categories (U-8, U-10, U-12, etc.)
- Divisions (Primera, Amateur, etc.)

**Key Entities:** `Position`, `Skill`, `Category`, `Division`

### 6. Asset Management

- Equipment and uniform tracking
- Inventory management with reorder points
- Asset allocation and check-out system
- Maintenance scheduling and cost tracking
- Depreciation calculations and financial reporting

**Key Entities:** `Asset`, `AssetCategory`, `AssetAllocation`  
**Key Endpoints:** `RegisterAssetEndpoint`, `AllocateAssetEndpoint`, `SearchAssetsEndpoint`

### 7. Reporting & Analytics

- Financial reports (P&L, revenue, expenses)
- Player development analytics
- Team performance metrics
- Operational efficiency reports
- Business intelligence and benchmarking

**Key Entities:** `Report`, `ReportGeneration`, `FinancialTransaction`  
**Key Endpoints:** `GenerateReportEndpoint`, `GetFinancialSummaryEndpoint`, `GetPlayerAnalyticsEndpoint`

### 8. Communication System

- Multi-channel messaging (email, SMS, push notifications)
- Parent portal with secure access
- Event notifications and reminders
- Emergency alerts and announcements
- Message delivery tracking and templates

**Key Entities:** `Message`, `MessageDelivery`, `ParentPortalAccess`  
**Key Endpoints:** `SendMessageEndpoint`, `CreateParentPortalAccessEndpoint`, `GetMessagesEndpoint`

## Key Features

### Multi-Academy Context Management (Hybrid Approach)

1. **URL-Based:** `/api/v1/{academySlug}/players`
2. **Header-Based:** `X-Academy-Context: academy-slug`
3. **Redux Store:** Frontend state management
4. **React Context:** Deep component tree access
5. **TanStack Query:** Automatic cache invalidation on academy switch

### Authentication & Authorization

- JWT-based authentication with ASP.NET Core Identity
- Policy-based authorization with custom requirements
- Academy-scoped permissions using custom authorization handlers
- System admin vs academy admin roles

### Real-Time Features (SignalR)

- WebSocket support for real-time updates
- Live training attendance updates
- Real-time notifications
- Hub methods for client-server communication

### Data Privacy & Security

- Data Protection API for sensitive data encryption
- Entity Framework encryption for specific columns
- Custom converters for encrypted properties
- Audit logging for critical operations

### Background Processing

- Hangfire for background jobs and scheduling
- Welcome emails on player registration
- Training reminders (24h and 2h before)
- Data exports and reports
- Recurring jobs with cron expressions

### API Features

- Pagination using PagedList or custom pagination classes
- Filtering and search using LINQ and specification pattern
- Relationship inclusion using OData-style query parameters
- Sorting with `orderBy` parameter
- Comprehensive error responses using `ProblemDetails`

## Development Workflow

### Running the Application

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run migrations
dotnet ef database update

# Run application
dotnet run --project src/Diquis.Api

# Run Hangfire server (separate process or same app)
# Hangfire runs in-process with the API by default

# Run tests
dotnet test
```

### Environment Structure

- **Development:** Local PostgreSQL/SQL Server + Redis
- **Test:** In-memory database or test database with xUnit
- **Production:** Dockerized with Kubernetes or Azure App Service

## Project Structure

```text
Diquis/
├── src/
│   ├── Diquis.Domain/              # Domain entities and interfaces
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Interfaces/
│   │   └── Common/
│   ├── Diquis.Application/         # Business logic and services
│   │   ├── Services/
│   │   │   ├── AcademyManagement/
│   │   │   ├── PlayerManagement/
│   │   │   ├── TeamManagement/
│   │   │   ├── TrainingManagement/
│   │   │   └── Common/
│   │   ├── DTOs/
│   │   ├── Validators/
│   │   └── Interfaces/
│   ├── Diquis.Infrastructure/      # Infrastructure layer
│   │   ├── Data/
│   │   │   ├── Configurations/
│   │   │   ├── Migrations/
│   │   │   └── Repositories/
│   │   ├── Identity/
│   │   ├── Services/
│   │   ├── Storage/
│   │   └── BackgroundJobs/
│   └── Diquis.Api/                 # FastEndpoints API
│       ├── Features/
│       │   ├── AcademyManagement/
│       │   │   ├── Create/
│       │   │   │   ├── CreateAcademyEndpoint.cs
│       │   │   │   ├── CreateAcademyRequest.cs
│       │   │   │   └── CreateAcademyResponse.cs
│       │   │   ├── Get/
│       │   │   └── Update/
│       │   ├── PlayerManagement/
│       │   ├── TeamManagement/
│       │   ├── TrainingManagement/
│       │   └── SharedResources/
│       ├── Middleware/
│       ├── Filters/
│       ├── Extensions/
│       └── Program.cs
├── tests/
│   ├── Diquis.Domain.Tests/
│   ├── Diquis.Application.Tests/
│   ├── Diquis.Infrastructure.Tests/
│   └── Diquis.Api.Tests/
└── docs/                              # Documentation
```

## Reference Implementation

This ASP.NET Core implementation is based on the Ruby on Rails "Diquis" project that serves as the architectural reference. The ASP.NET version maintains the same business logic, domain models, and API contracts while leveraging .NET conventions and best practices.

## Getting Started

See the following documentation files:

- [SETUP_GUIDE.md](./SETUP_GUIDE.md) - Initial project setup
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Detailed architecture documentation
- [API_AUTHENTICATION.md](./API_AUTHENTICATION.md) - Authentication implementation
- [AUTHORIZATION.md](./AUTHORIZATION.md) - Authorization setup
- [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Development workflows

## NuGet Package Management

### Package Restore

```bash
# Restore all packages
dotnet restore

# Add new package
dotnet add package PackageName --version x.x.x

# Update package
dotnet add package PackageName --version x.x.x

# Remove package
dotnet remove package PackageName
```

### Key Packages

- **FastEndpoints:** Modern, high-performance endpoint library
- **Microsoft.EntityFrameworkCore:** ORM for database access
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore:** Authentication
- **Microsoft.AspNetCore.Authentication.JwtBearer:** JWT support
- **Hangfire** or **Quartz.NET:** Background jobs (choose one)
- **FluentValidation:** Request validation
- **Mapster** or **AutoMapper:** Object mapping (Mapster is faster)
- **Serilog:** Structured logging
- **StackExchange.Redis:** Redis caching
- **xUnit:** Testing framework
- **Moq:** Mocking framework
- **FluentAssertions:** Test assertions
- **Testcontainers:** Integration testing with containers

## C# Language Features

### Modern C# 12 Features Used

- **Primary constructors** for dependency injection
- **Collection expressions** for initialization
- **Pattern matching** for cleaner code
- **Nullable reference types** for null safety
- **Record types** for DTOs and value objects
- **Global usings** for common namespaces
- **File-scoped namespaces** for cleaner code

### Example Code Style

**FastEndpoints Endpoint:**

```csharp
// Request DTO
public sealed record CreatePlayerRequest
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public int Age { get; init; }
    public string Gender { get; init; } = default!;
    public Guid PositionId { get; init; }
    public Guid CategoryId { get; init; }
}

// Response DTO
public sealed record CreatePlayerResponse(
    Guid Id,
    string FirstName,
    string LastName,
    int Age,
    DateTime CreatedAt);

// Validator
public sealed class CreatePlayerValidator : Validator<CreatePlayerRequest>
{
    public CreatePlayerValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);
            
        RuleFor(x => x.Age)
            .GreaterThan(4)
            .LessThan(100);
            
        RuleFor(x => x.Gender)
            .Must(g => new[] { "M", "F", "NB", "PNTS" }.Contains(g))
            .WithMessage("Invalid gender value");
    }
}

// Endpoint
public sealed class CreatePlayerEndpoint(
    IPlayerService playerService,
    ILogger<CreatePlayerEndpoint> logger) 
    : Endpoint<CreatePlayerRequest, CreatePlayerResponse>
{
    public override void Configure()
    {
        Post("/api/v1/{academySlug}/players");
        Policies("CreatePlayer"); // Authorization policy
        Description(b => b
            .WithTags("Player Management")
            .WithSummary("Register a new player")
            .Produces<CreatePlayerResponse>(201)
            .ProducesProblemDetails(400)
            .ProducesProblemDetails(401));
    }

    public override async Task HandleAsync(
        CreatePlayerRequest req, 
        CancellationToken ct)
    {
        try
        {
            var academySlug = Route<string>("academySlug");
            
            var player = await playerService.CreatePlayerAsync(
                academySlug, 
                req, 
                ct);
            
            var response = new CreatePlayerResponse(
                player.Id,
                player.FirstName,
                player.LastName,
                player.Age,
                player.CreatedAt);
            
            await SendCreatedAtAsync<GetPlayerEndpoint>(
                new { id = player.Id },
                response,
                cancellation: ct);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation failed for player creation");
            ThrowError(ex.Message, 400);
        }
    }
}
```

**Service with Primary Constructor:**

```csharp
public sealed class PlayerService(
    ApplicationDbContext dbContext,
    IMapper mapper,
    ILogger<PlayerService> logger) : IPlayerService
{
    public async Task<Player> CreatePlayerAsync(
        string academySlug,
        CreatePlayerRequest request,
        CancellationToken ct = default)
    {
        // Tenant context is automatically set via middleware
        var player = mapper.Map<Player>(request);
        
        dbContext.Players.Add(player);
        await dbContext.SaveChangesAsync(ct);
        
        logger.LogInformation(
            "Player {PlayerId} created in academy {AcademySlug}",
            player.Id,
            academySlug);
        
        return player;
    }
}
```

## License

MIT License

## Contact

Development Team: dev@diquis.com
