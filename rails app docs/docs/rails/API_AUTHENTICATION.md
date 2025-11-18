# API Authentication Documentation

This document describes the authentication system for the Diquis Football API.

## Overview

The API uses **Devise** for user authentication combined with **JWT (JSON Web Tokens)** for API token handling.
This provides a secure, stateless authentication mechanism suitable for API-only applications.

## Authentication Endpoints

### Sign Up (Register)

Create a new user account.

**Endpoint:** `POST /auth/sign_up`

**Request Headers:**

```http
Content-Type: application/json
```

**Request Body:**

```json
{
  "user": {
    "email": "user@example.com",
    "password": "password123",
    "password_confirmation": "password123",
    "first_name": "John",
    "last_name": "Doe"
  }
}
```

**Success Response (201 Created):**

```json
{
  "data": {
    "id": 1,
    "email": "user@example.com",
    "first_name": "John",
    "last_name": "Doe"
  },
  "message": "Signed up successfully"
}
```

**Response Headers:**

```http
Authorization: Bearer <jwt_token>
```

**Error Response (422 Unprocessable Entity):**

```json
{
  "message": "Sign up failed",
  "errors": [
    "Email has already been taken",
    "Password is too short (minimum is 6 characters)"
  ]
}
```

---

### Sign In (Login)

Authenticate an existing user and receive a JWT token.

**Endpoint:** `POST /auth/sign_in`

**Request Headers:**

```http
Content-Type: application/json
```

**Request Body:**

```json
{
  "user": {
    "email": "user@example.com",
    "password": "password123"
  }
}
```

**Success Response (200 OK):**

```json
{
  "data": {
    "id": 1,
    "email": "user@example.com",
    "first_name": "John",
    "last_name": "Doe",
    "is_system_admin": false
  },
  "message": "Logged in successfully"
}
```

**Response Headers:**

```http
Authorization: Bearer <jwt_token>
```

**Error Response (401 Unauthorized):**

```json
{
  "error": "Invalid Email or password."
}
```

---

### Sign Out (Logout)

Invalidate the current JWT token and log out the user.

**Endpoint:** `DELETE /auth/sign_out`

**Request Headers:**

```http
Content-Type: application/json
Authorization: Bearer <jwt_token>
```

**Success Response (200 OK):**

```json
{
  "message": "Logged out successfully"
}
```

**Error Response (401 Unauthorized):**

```json
{
  "message": "No active session"
}
```

---

## Making Authenticated Requests

To access protected API endpoints, include the JWT token in the Authorization header:

```http
Authorization: Bearer <jwt_token>
```

**Example:**

```bash
curl -X GET "https://api.example.com/api/v1/protected_resource" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiJ9..." \
  -H "Content-Type: application/json"
```

---

## Token Management

### Token Lifetime

JWT tokens expire after **24 hours** by default. After expiration, users must sign in again to obtain a new token.

### Token Revocation

When a user signs out, their JWT token is added to a denylist (`jwt_denylists` table) to prevent further use.
This ensures that logged-out tokens cannot be reused even if they haven't expired yet.

---

## Security Features

1. **Password Encryption**: User passwords are encrypted using bcrypt with configurable cost factor
   (12 in production, 1 in test for performance).

2. **JWT Secret Key**: Tokens are signed using a secret key stored in Rails credentials or environment variables:
   - `Rails.application.credentials.devise_jwt_secret_key`
   - `ENV['DEVISE_JWT_SECRET_KEY']`

3. **Token Denylist**: Revoked tokens are stored in the database to prevent reuse.

4. **HTTPS Required**: In production, all authentication requests should be made over HTTPS.

5. **UUID Slugs**: Each user has a unique UUID slug for additional security and to avoid exposing sequential IDs.

---

## Error Handling

The API returns standardized error responses:

### 401 Unauthorized

Returned when authentication fails or token is invalid/expired.

### 422 Unprocessable Entity

Returned when request validation fails (e.g., missing required fields, invalid email format).

### 500 Internal Server Error

Returned when an unexpected server error occurs.

---

## Configuration

### Generate JWT Secret Key

To generate a new secret key for JWT tokens:

```bash
# Generate a random secret
rake secret

# Add to Rails credentials
rails credentials:edit

# Add the following line:
# devise_jwt_secret_key: <paste_the_generated_secret_here>
```

### Environment Variables

Alternatively, you can set the JWT secret key as an environment variable:

```bash
export DEVISE_JWT_SECRET_KEY="your_secret_key_here"
```

---

## User Model Fields

The User model includes the following fields:

- `id` (integer): Primary key
- `email` (string): User's email address (unique, required)
- `encrypted_password` (string): Encrypted password
- `slug` (uuid): Unique UUID identifier
- `first_name` (string): User's first name (optional)
- `last_name` (string): User's last name (optional)
- `is_system_admin` (boolean): System admin flag (default: false)
- `reset_password_token` (string): Token for password recovery
- `reset_password_sent_at` (datetime): When password reset was requested
- `remember_created_at` (datetime): When "remember me" was created
- `created_at` (datetime): Record creation timestamp
- `updated_at` (datetime): Record update timestamp

---

## Testing Authentication

### Using cURL

**Sign Up:**

```bash
curl -X POST "http://localhost:3000/auth/sign_up" \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "test@example.com",
      "password": "password123",
      "password_confirmation": "password123",
      "first_name": "Test",
      "last_name": "User"
    }
  }'
```

**Sign In:**

```bash
curl -X POST "http://localhost:3000/auth/sign_in" \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "test@example.com",
      "password": "password123"
    }
  }' \
  -i  # Include headers to see the Authorization token
```

**Sign Out:**

```bash
curl -X DELETE "http://localhost:3000/auth/sign_out" \
  -H "Authorization: Bearer <your_token_here>" \
  -H "Content-Type: application/json"
```

---

## Next Steps

- Implement password recovery endpoints
- Add email confirmation (optional)
- Implement account locking after failed attempts (optional)
- Add refresh token mechanism for long-lived sessions
- Implement role-based authorization with Pundit

---

## Related Documentation

- [Devise Documentation](https://github.com/heartcombo/devise)
- [Devise-JWT Documentation](https://github.com/waiting-for-dev/devise-jwt)
- [JWT.io](https://jwt.io/) - JWT introduction and debugger
