# ASP.NET - Multi-Tenancy Architecture

This document explains the multi-tenancy implementation in the Diquis application, which allows a single instance of the application to serve multiple organizations (tenants) with isolated data.

## Overview

The application supports a multi-database multi-tenancy model, where each tenant's data is stored in a separate, isolated database. A central `TenantDbContext` stores the tenant metadata, while each tenant has its own `ApplicationDbContext`.

## Key Components

### 1. Tenant Resolution

The current tenant is identified for each incoming HTTP request using the `TenantResolver` middleware.

-   **Middleware**: `TenantResolver.cs` inspects the request (e.g., URL, header, or user claim) to identify the tenant.
-   **Service**: `CurrentTenantUserService` is a scoped service that holds the `TenantId` and `UserId` for the duration of the request. This service is accessible throughout the application via dependency injection.

### 2. Database Contexts

The application uses multiple `DbContext` types to manage data isolation:

-   **`TenantDbContext`**:
    -   This context connects to a shared, central database.
    -   It manages the `Tenants` table and other shared entities.
    -   It is used for tenant creation, lookup, and management.

-   **`ApplicationDbContext`**:
    -   This is the main context for tenant-specific data (e.g., Players, Teams).
    -   The connection string for this context is resolved dynamically at runtime based on the current tenant identified by the `CurrentTenantUserService`.
    -   It includes global query filters to ensure that data is always filtered by the current `AcademyId` (TenantId), providing an extra layer of data isolation.

-   **`BaseDbContext`**:
    -   A base class that both `TenantDbContext` and `ApplicationDbContext` inherit from.
    -   It overrides `SaveChanges` and `SaveChangesAsync` to automatically set audit properties (`CreatedBy`, `LastModifiedBy`, etc.) on entities.

### 3. Data Isolation and Query Filters

Even within a tenant's database, global query filters are applied to all entities that implement the `IMustHaveAcademy` interface. This ensures that every query is automatically filtered by the current `AcademyId`, preventing any possibility of data leakage between tenants in a single-database scenario or adding defense-in-depth in a multi-database scenario.

### 4. Tenant Management

-   **`TenantsController`**: An API controller for managing tenants (creating, activating/deactivating, etc.). Access to this controller is typically restricted to system administrators.
-   **`TenantManagementService`**: An application service that encapsulates the business logic for tenant management.

### 5. Database Migrations

-   **`TenantDbContext` Migrations**: These migrations are applied to the shared central database.
-   **`ApplicationDbContext` Migrations**: These migrations must be applied to each tenant's individual database. The application includes logic to automatically apply pending migrations to a tenant's database when the tenant is first accessed.

This multi-layered approach ensures robust data isolation and provides a scalable architecture for a SaaS application.
