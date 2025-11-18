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
**Goal:** Implement player registration, profiles, and search.

### Phase 4: Team Management (Week 4-5)
**Goal:** Implement team organization and player assignments.

### Phase 5: Training Management (Week 5-6)
**Goal:** Implement training scheduling and attendance tracking.

### Phase 6: Shared Resources (Week 6)
**Goal:** Create models for positions, skills, and categories.

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
