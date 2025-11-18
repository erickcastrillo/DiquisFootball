# Merge Conflict Resolution

This document describes how merge conflicts between a feature branch (e.g., authentication) and the main development branch were resolved.

## Conflicts Resolved

### 1. `Program.cs` or `Startup.cs`

-   **Conflict**: The main branch added services for multi-tenancy, while the feature branch added services for authentication and authorization (e.g., `AddIdentity`, `AddAuthentication`, `AddAuthorization`).
-   **Resolution**: Merged both sets of service registrations, ensuring the correct order. Authentication and Authorization services are typically registered before application-specific services. The middleware pipeline was also merged, ensuring `app.UseAuthentication()` and `app.UseAuthorization()` are placed correctly (after routing but before endpoints).

### 2. Base Controller or Middleware

-   **Conflict**: The main branch added logic for resolving the tenant ID, while the feature branch added logic for handling the authenticated user's context.
-   **Resolution**: The logic was combined. The middleware pipeline was ordered so that authentication runs first, allowing the tenant resolution logic to potentially use the authenticated user's claims to identify the tenant.

### 3. Test Fixtures or Factories

-   **Conflict**: The main branch added test helpers for creating tenants (`Academy`), while the feature branch added helpers for creating users (`ApplicationUser`).
-   **Resolution**: Both sets of test helpers were combined into shared testing projects.

## Result

The authentication system now works harmoniously with the multi-tenancy system:

-   Users can register and log in without needing a tenant context.
-   Once authenticated, API requests to tenant-scoped endpoints are validated to ensure the user has access to the requested tenant.
-   Authentication endpoints (e.g., `/api/tokens`) do not require tenant scoping.
-   All other business-logic endpoints require both authentication and a valid tenant context.
