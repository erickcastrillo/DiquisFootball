# Diquis - ASP.NET Core Project Overview

**Project Name:** Diquis Football Management System
**Template:** ASPNano (ASP.NET Nano Boilerplate)
**Framework:** ASP.NET Core 8.0+
**Architecture:** Clean Architecture

## Project Philosophy

Diquis is built upon the **ASPNano Boilerplate**, a project template designed to scaffold best-practice, production-ready ASP.NET Core applications.

It's important to understand that this is a **template, not a framework**. It provides simple, generic, and customizable code with minimal abstraction. The goal is to accelerate the setup of new projects by providing a solid architectural foundation and common features out-of-the-box, without imposing a rigid structure that is difficult to change.

## Core Business Domains

Diquis is a comprehensive football (soccer) academy management system designed to manage:
- **Academy Management**: Handles the creation, configuration, and management of tenants (academies).
- **Player Management**: Manages player profiles, registration, and data.
- **Team Management**: Organizes players into teams and manages rosters.
- **Training Management**: Schedules and tracks training sessions and player attendance.
- **Asset Management**: Tracks and manages academy equipment and resources.

## Architecture Principles

### 1. Clean Architecture

The application follows Clean Architecture principles to create a strong separation of concerns, making the system easier to maintain, test, and scale.

- **`Diquis.Domain`**: Contains the core business logic, including entities, enums, and domain events. This layer has no dependencies on any other layer.
- **`Diquis.Application`**: Orchestrates the business logic by using domain entities. It defines interfaces for infrastructure concerns (like repositories and services) but does not implement them. It depends only on the Domain layer.
- **`Diquis.Infrastructure`**: Implements the interfaces defined in the Application layer. This includes data access (Entity Framework Core repositories), file storage, and other external services.
- **`Diquis.WebApi`**: The presentation layer that exposes the application's functionality through a RESTful API. It handles HTTP requests, authentication, and authorization.

### 2. Key Patterns & Practices

- **Generic Repository & Specification Pattern**: Decouples data access logic from business logic, allowing for reusable and composable queries.
- **Multi-Tenant Design**: Each academy is a tenant, with data isolated in a separate database. A `TenantResolver` middleware and tenant-aware data contexts handle data scoping automatically.
- **API-First Design**: The system is designed to be consumed by various clients (web, mobile), with automatic OpenAPI/Swagger documentation.
- **User & Identity Management**: Built-in user management using ASP.NET Core Identity.

## Technology Stack

### Backend
- **Framework:** ASP.NET Core 8.0+
- **Language:** C# 12
- **Database:** PostgreSQL 15+ or SQL Server
- **ORM:** Entity Framework Core 8.0+
- **Authentication:** ASP.NET Core Identity + JWT
- **API Documentation:** Swashbuckle (Swagger)

### Frontend
- A separate frontend application built with **React** (using Vite) is provided.

## Getting Started

For detailed setup instructions, please refer to the [Setup Guide](./SETUP_GUIDE.md).
