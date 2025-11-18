# API Authentication (ASP.NET Core)

This document describes the authentication system for the Diquis API, which is built using ASP.NET Core Identity and JSON Web Tokens (JWT).

## Authentication Flow

The API uses a stateless, token-based authentication mechanism.

1.  **Login**: A user submits their credentials to the `TokensController`.
2.  **Token Generation**: The `AuthenticationService` validates the credentials and, if successful, generates a JWT and a refresh token.
3.  **Token Storage**: The client is responsible for securely storing the JWT and refresh token.
4.  **Authenticated Requests**: For subsequent requests, the client sends the JWT in the `Authorization` header.
5.  **Token Validation**: The ASP.NET Core JWT middleware validates the token on every request to authorize access.

## Core Authentication Services

The authentication system is composed of several key services that work together.

### 1. `IdentityService`

-   **Location**: `Diquis.Infrastructure/Identity/IdentityService.cs`
-   **Responsibilities**:
    -   Manages user and role creation, deletion, and updates.
    -   Handles password-related operations (e.g., password hashing, reset tokens).
    -   Provides an abstraction over the ASP.NET Core Identity `UserManager` and `RoleManager`.

### 2. `AuthenticationService`

-   **Location**: `Diquis.Infrastructure/Auth/JWT/AuthenticationService.cs`
-   **Responsibilities**:
    -   **Issues Tokens**: This is the primary service for generating JWT and refresh tokens.
    -   **Credential Validation**: It coordinates with the `IdentityService` to validate a user's email and password.
    -   **Claims Generation**: It gathers the necessary user claims (User ID, Tenant ID, roles, permissions) to be embedded in the JWT.

### 3. `CurrentTenantUserService`

-   **Location**: `Diquis.WebApi/Services/CurrentTenantUserService.cs`
-   **Responsibilities**:
    -   **Request-Scoped Context**: This is a scoped service that holds the `TenantId` and `UserId` for the duration of a single HTTP request.
    -   **Context Population**: It is populated by the `TenantResolver` middleware at the beginning of the request pipeline.
    -   **Centralized Access**: It provides a single, reliable source for accessing the current user and tenant context from anywhere in the application (e.g., from services or repositories) without needing to pass IDs down the call stack.

## Authentication Endpoints

### `POST /api/tokens`

Authenticates a user and returns a JWT and refresh token.

**Request Body:**
```json
{
  "email": "admin@admin.com",
  "password": "admin"
}
```

**Success Response (`200 OK`):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64_encoded_refresh_token",
  "expiresIn": 3600
}
```

### `POST /api/tokens/refresh`

Allows a client to get a new JWT using a valid refresh token.

## Making Authenticated Requests

To access protected API endpoints, include the JWT in the `Authorization` header with the `Bearer` scheme.

```http
GET /api/v1/products
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Configuration

The JWT settings (secret key, issuer, audience) are configured in `appsettings.json` and should be overridden for production using a secure mechanism like User Secrets or Azure Key Vault.

**`appsettings.json`:**
```json
{
  "JwtSettings": {
    "Secret": "A_VERY_LONG_AND_SECURE_SECRET_KEY_GOES_HERE",
    "Issuer": "DiquisApi",
    "Audience": "DiquisApiClient",
    "ExpiryInMinutes": 60
  }
}
```

The JWT middleware and Identity services are configured in `Program.cs`.
