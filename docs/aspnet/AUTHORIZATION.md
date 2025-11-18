# Authorization in ASP.NET Core

This document describes how authorization is implemented in the Diquis API using ASP.NET Core's role-based and policy-based systems.

## Overview

Authorization is the process of determining whether an authenticated user has permission to perform an action. This happens *after* a user has been successfully authenticated. The primary way to control access in the API is by using the `[Authorize]` attribute on controllers and action methods.

## Role-Based Authorization

The simplest form of authorization is based on roles. The system defines a set of roles (e.g., `Admin`, `Coach`, `Player`), and you can restrict access to endpoints based on the user's assigned role.

**Example: Restricting an endpoint to Admins:**
```csharp
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    // Endpoints in this controller are only accessible by users 
    // in the "Admin" role.
}
```

## Policy-Based Authorization

For more complex scenarios, we use policies. A policy is a set of requirements that a user must meet to be granted access. Policies are defined once in `Program.cs` and can be reused throughout the application.

**Example: A policy that requires a user to be an Admin OR a Coach:**

**1. Define the Policy (in `Program.cs`):**
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrCoach", policy =>
        policy.RequireRole("Admin", "Coach"));
});
```

**2. Use the Policy (in a controller):**
```csharp
[HttpGet("team-roster")]
[Authorize(Policy = "AdminOrCoach")]
public IActionResult GetTeamRoster()
{
    // This endpoint is accessible to both Admins and Coaches.
}
```

## Custom Authorization Handlers

For business logic that goes beyond simple role checks (e.g., "Is this user a member of the academy they are trying to access?"), we can create custom authorization handlers.

-   **Requirement**: A class that defines the data needed for the authorization check.
-   **Handler**: A class that contains the logic to evaluate the requirement.

The handler inspects the user's claims and other request context (like URL parameters) to determine if the user meets the requirements. If the requirements are met, access is granted; otherwise, it is denied with a `403 Forbidden` response.

This powerful mechanism is used to enforce tenant isolation, ensuring a user from one academy cannot access the data of another.

## Best Practices

-   **Deny by Default**: Always secure endpoints with `[Authorize]` by default and explicitly grant access where needed.
-   **Use Policies**: Prefer policies over roles for more maintainable and flexible authorization logic.
-   **Centralize Logic**: Keep complex authorization logic within handlers rather than scattering it in controllers.
-   **Test Authorization**: Write specific tests for your authorization policies to prevent security vulnerabilities.
