# Diquis - ASP.NET Core API

> A comprehensive ASP.NET Core 8.0+ API for managing football academies, players, teams, and training sessions with a multi-tenant architecture.

[![.NET Build](https://github.com/erickcastrillo/Diquis/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/erickcastrillo/Diquis/actions/workflows/dotnet-build.yml)
[![C# Version](https://img.shields.io/badge/C%23-12-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-blue.svg)](https://www.postgresql.org/)

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Quick Start](#quick-start)
- [API Documentation](#api-documentation)
- [Technology Stack](#technology-stack)
- [Project Status](#project-status)

## 🎯 Overview

Diquis is a modern football academy management system built with an ASP.NET Core 8.0+ API-only architecture. It implements a **Clean Architecture** approach with complete multi-tenant isolation, allowing multiple academies to operate independently within a single application.

The system manages:

- 🏫 **Academy Operations** - Complete academy administration
- ⚽ **Player Management** - Player registration, detailed profiles (PlayerProfile), and skill tracking
- 👥 **Team Organization** - Team rosters, memberships, and division assignments (Team, Division)
- 📅 **Training Sessions** - Scheduling and attendance tracking
- 📊 **Analytics & Reporting** - Performance metrics and business intelligence
- 🎽 **Asset Management** - Equipment, uniforms, and inventory tracking

## ✨ Features

### Multi-Tenant Architecture
- **Complete Data Isolation** - Each academy's data is fully isolated using tenant-aware repositories.
- **Hybrid Context Management** - Tenant context resolved from URL, headers, or user claims.
- **Cross-Academy Administration** - System admin access across all academies.

### Advanced Player Management
- Player registration with detailed profiles.
- Age-category validation.
- Position and skill assignments.
- Player search and filtering.
- Image upload support via Azure Blob Storage or local storage.

### API Features
- **RESTful Design** - Clean, predictable API endpoints.
- **JWT Authentication** - Secure, token-based authentication with ASP.NET Core Identity.
- **Role-Based Authorization** - Policy-based authorization for fine-grained access control.
- **Pagination** - Efficient data pagination with metadata.
- **OpenAPI Documentation** - Interactive Swagger UI for all endpoints.

### Developer Experience
- **CI/CD Pipeline** - Automated testing and building with GitHub Actions.
- **Comprehensive Testing** - xUnit for unit and integration tests.
- **Code Quality Tools** - .NET format, StyleCop, and Roslyn Analyzers.
- **Background Jobs** - Hangfire or Quartz.NET for asynchronous tasks.
- **Development Documentation** - Complete guides and examples in the `/docs` directory.

## 🏗️ Architecture

Diquis is built using **Clean Architecture** principles, promoting separation of concerns and maintainability.

- **`Diquis.Domain`**: Contains core business entities, enums, and domain-specific logic. It has no dependencies on other layers.
- **`Diquis.Application`**: Implements the business logic and use cases. It defines interfaces for repositories and other infrastructure concerns. It depends only on the Domain layer.
- **`Diquis.Infrastructure`**: Provides data access and external service implementations (e.g., EF Core repositories, JWT services, file storage). It depends on the Application layer.
- **`Diquis.WebApi`**: The presentation layer, exposing the application's features via a RESTful API. It depends on the Application and Infrastructure layers.

### Key Design Patterns
1. **Repository Pattern** - Decouples business logic from data access.
2. **CQRS (Command Query Responsibility Segregation)** - Can be implemented for more complex features to separate read and write operations.
3. **Multi-Tenant Pattern** - Academy-based data isolation.
4. **Policy-Based Authorization** - For flexible and maintainable access control.

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- PostgreSQL 15+ or SQL Server
- A C# IDE (Visual Studio, VS Code, Rider)

### Installation
```bash
# 1. Clone the repository
git clone https://github.com/erickcastrillo/Diquis.git
cd Diquis

# 2. Configure user secrets
# (In Diquis.WebApi directory)
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your_connection_string"
dotnet user-secrets set "JwtSettings:Secret" "your_super_secret_jwt_key_that_is_long_enough"

# 3. Install dependencies
dotnet restore

# 4. Generate and apply database migrations
# Generate a new migration (replace 'InitialMigration' with a descriptive name)
dotnet ef migrations add InitialMigration --project Diquis.Infrastructure --startup-project Diquis.WebApi --context ApplicationDbContext -o Persistence/Migrations/AppDb

# Apply pending migrations to the database
dotnet ef database update --project Diquis.Infrastructure

# 5. Run the application
dotnet run --project Diquis.WebApi
```

### Access Points
- **API:** `https://localhost:5001` (or `http://localhost:5000`)
- **API Documentation:** `https://localhost:5001/swagger`

## 📚 Documentation

Comprehensive documentation is available in the `/docs` directory.

- **[Project Overview](./docs/aspnet/PROJECT_OVERVIEW.md)**
- **[Architecture Guide](./docs/aspnet/ARCHITECTURE.md)**
- **[Setup Guide](./docs/aspnet/SETUP_GUIDE.md)**
- **[API Authentication](./docs/aspnet/API_AUTHENTICATION.md)**
- **[Development Guide](./docs/aspnet/DEVELOPMENT_GUIDE.md)**

## 🛠️ Technology Stack

### Backend
- **Framework:** ASP.NET Core 8.0
- **Language:** C# 12
- **Database:** PostgreSQL 15+ / SQL Server
- **ORM:** Entity Framework Core 8
- **Authentication:** ASP.NET Core Identity + JWT
- **Authorization:** Policy-Based Authorization
- **Background Jobs:** Hangfire or Quartz.NET
- **API Documentation:** Swashbuckle (Swagger)
- **Observability:** OpenTelemetry

### Development Tools
- **Testing:** xUnit, Moq, FluentAssertions
- **Code Quality:** .NET Analyzers, StyleCop
- **Containerization:** Docker

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test Diquis.Application.Tests/

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Project Status

### Current Implementation
✅ Project structure and documentation
✅ Clean Architecture foundation
✅ Multi-tenancy configuration
✅ Authentication setup (Identity + JWT)
✅ Authorization setup (Policy-based)
✅ CI/CD pipeline for builds

### Planned
📋 Player management features (including PlayerProfile)
📋 Team management features (including Team and Division)
📋 Training management features
📋 Frontend React application
📋 Real-time features with SignalR
📋 Email notification system
📋 Report generation
📋 Analytics dashboard

## 🤝 Contributing

We welcome contributions! Please see `CONTRIBUTING.md` for details on how to get started.

## 📝 License

This project is licensed under the MIT License - see the `LICENSE` file for details.
