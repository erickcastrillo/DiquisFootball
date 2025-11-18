# Diquis - Phased Implementation Plan (.NET)

## Overview

This document outlines a systematic, phase-by-phase approach to implementing the Diquis ASP.NET Core API. Each phase builds upon the previous one, with clear deliverables and verification steps.

---

## 📋 Implementation Phases

### Phase 0: Project Foundation (Week 1)
**Goal:** Set up the development environment and solution structure.

### Phase 1: Core Infrastructure (Week 1-2)
**Goal:** Implement base classes, multi-tenancy, and authentication.

### Phase 2: Academy Management (Week 2-3)
**Goal:** Complete academy CRUD operations and user management.

### Phase 3: Player Management (Week 3-4)
**Goal:** Implement player registration, detailed profiles using `PlayerProfile`, and search.

### Objectives
- Create the `PlayerProfile` entity to store player-specific details.
- Establish the relationship between `ApplicationUser` and `PlayerProfile`.
- Implement CRUD operations for `PlayerProfile`.
- Implement services for managing player profiles.
- Create API endpoints for player management.

### Tasks
1.  **Create Entities**: Define `PlayerProfile` in `Diquis.Domain` with properties like `DateOfBirth`, `Position`, `JerseyNumber`, `Height`, `Weight`, `PreferredFoot`, `EmergencyContactName`, `EmergencyContactPhone`, `MedicalNotes`, `RegistrationDate`.
2.  **Establish Relationships**: Configure the one-to-one relationship between `ApplicationUser` and `PlayerProfile` in EF Core.
3.  **Create Migrations**: Use `dotnet ef migrations add` to create the `PlayerProfile` table.
4.  **Implement Services**: Create `PlayerProfileService` in `Diquis.Application`.
5.  **Create Controllers/Endpoints**: Expose the functionality in `Diquis.WebApi`.
6.  **Add Authorization Policies**: Secure the endpoints.
7.  **Write Tests**: Add unit and integration tests.

### Phase 4: Team Management (Week 4-5)
**Goal:** Implement team organization, division assignments, and player assignments.

### Objectives
- Create `Team` and `Division` entities.
- Establish relationships between `Team`, `Division`, and `PlayerProfile`.
- Implement CRUD operations for `Team` and `Division`.
- Implement services for managing teams and divisions.
- Create API endpoints for team and division management.

### Tasks
1.  **Create Entities**: Define `Team` (with properties like `Name`, `Description`, `CoachId`) and `Division` (with properties like `Name`, `Description`, `MinAge`, `MaxAge`) in `Diquis.Domain`.
2.  **Establish Relationships**: Configure relationships between `Team` and `Division` (one-to-many), and `Team` and `PlayerProfile` (many-to-many or one-to-many depending on design).
3.  **Create Migrations**: Use `dotnet ef migrations add` to create the `Team` and `Division` tables.
4.  **Implement Services**: Create `TeamService` and `DivisionService` in `Diquis.Application`.
5.  **Create Controllers/Endpoints**: Expose the functionality in `Diquis.WebApi`.
6.  **Add Authorization Policies**: Secure the endpoints.
7.  **Write Tests**: Add unit and integration tests.

### Phase 5: Training Management (Week 5-6)
**Goal:** Implement training scheduling and attendance tracking.

### Phase 6: Shared Resources (Week 6)
**Goal:** Create models for positions, skills, and categories.

### Objectives
- Utilize the existing `Category` model for various classifications.
- Define and implement other shared resource models as needed (e.g., `Position`, `Skill`).

### Tasks
1.  **Utilize `Category` Model**: Ensure the `Category` model (`Diquis.Domain\Entities\Football\Common\Category.cs`) is integrated and used for classification where appropriate (e.g., player skill categories, team age categories).
2.  **Create Other Shared Entities**: Define `Position` and `Skill` entities in `Diquis.Domain` if they are distinct domain concepts.
3.  **Create Migrations**: Use `dotnet ef migrations add` for any new shared entities.
4.  **Implement Services**: Create services for managing these shared resources.
5.  **Create Controllers/Endpoints**: Expose the functionality in `Diquis.WebApi`.
6.  **Write Tests**: Add unit and integration tests.

### Phase 7: Testing & Documentation (Week 7)
**Goal:** Ensure comprehensive test coverage and generate API documentation.

### Phase 8: Production Readiness (Week 8)
**Goal:** Optimize for performance and prepare for deployment.

### Phase 9-12: Advanced Features
-   Asset Management
-   Reporting & Analytics
-   Communication System
-   Health Management & Events

---

## Phase 0: Project Foundation

### Objectives
- Install all required dependencies (.NET SDK, Database).
- Create the solution and project structure (Domain, Application, Infrastructure, WebApi).
- Configure the database connection and other essential services.
- Set up development tools (e.g., EF Core tools, user secrets).
- Initialize the Git repository.

### Tasks
1.  **Environment Setup**: Install .NET 8 SDK, PostgreSQL/SQL Server, and an IDE.
2.  **Create Solution**: Use `dotnet new sln` and `dotnet new` templates to create the layered architecture.
3.  **Install NuGet Packages**: Add core packages like EF Core, ASP.NET Core Identity, and JWT Bearer.
4.  **Configure `Program.cs`**: Set up dependency injection, authentication, and the middleware pipeline.
5.  **Initialize Git**: Create the repository and a `.gitignore` file for .NET projects.

### Verification
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project Diquis.WebApi

# Check the health endpoint
curl -k https://localhost:5001/health
```

---

## Phase 1: Core Infrastructure

### Objectives
- Implement base classes (e.g., `BaseEntity`, `IRepository<T>`).
- Set up multi-tenancy middleware and query filters.
- Configure ASP.NET Core Identity with JWT for authentication.
- Configure policy-based authorization.
- Set up background jobs with Hangfire or Quartz.NET.

### Tasks
1.  **Database Setup**: Create an initial migration to enable UUID support if using PostgreSQL (`porygrypto`).
2.  **Create `ApplicationUser`**: Extend `IdentityUser` with custom fields.
3.  **Implement JWT Service**: Create a service to generate JWTs.
4.  **Configure Auth**: Set up authentication and authorization services in `Program.cs`.
5.  **Implement Base Repository**: Create a generic repository for data access.
6.  **Implement Tenant Middleware**: Create middleware to resolve the tenant for each request.

### Verification
```bash
# Run database migrations
dotnet ef database update --project Diquis.Infrastructure

# Test authentication endpoints
curl -k -X POST https://localhost:5001/api/tokens -d '{"email": "...", "password": "..."}'
```

---

## Phase 2: Academy Management

### Objectives
- Create the `Academy` (tenant) and `AcademyUser` entities.
- Implement CRUD operations for academies.
- Implement services for managing academies and user roles within them.
- Create API endpoints for academy management.

### Tasks
1.  **Create Entities**: Define `Academy` and `AcademyUser` in `Diquis.Domain`.
2.  **Create Migrations**: Use `dotnet ef migrations add` to create the tables.
3.  **Implement Services**: Create `AcademyService` in `Diquis.Application`.
4.  **Create Controllers/Endpoints**: Expose the functionality in `Diquis.WebApi`.
5.  **Add Authorization Policies**: Secure the endpoints.
6.  **Write Tests**: Add unit and integration tests.

### Verification
```bash
# Run tests for the academy feature
dotnet test --filter "FullyQualifiedName~Academy"

# Test API endpoints via Swagger or curl
curl -k -H "Authorization: Bearer <token>" https://localhost:5001/api/v1/academies
```

---

(Phases 3-12 would follow a similar pattern of defining objectives, tasks, and verification steps for each feature area, adapted to the .NET technology stack.)
