# Seed Data Guide (.NET)

This document explains how to use the seed data in the Diquis API application for development and testing.

## Overview

The seed data creates a complete set of test users, academies, and related data to populate a development database. This allows for immediate testing of the API with realistic data representing all major roles and entities.

## Password Configuration

For security, all seeded users are created with a configurable default password.

### Setting the Password

You can configure the default seed password in your `appsettings.Development.json` or through user secrets.

**1. `appsettings.Development.json` (Recommended for local development):**
```json
{
  "SeedSettings": {
    "DefaultPassword": "YourSecurePassword!123"
  }
}
```

**2. User Secrets:**
```bash
# Navigate to the WebApi project
cd Diquis.WebApi

# Set the secret
dotnet user-secrets set "SeedSettings:DefaultPassword" "YourSecurePassword!123"
```

If neither is set, the seeder will use a hardcoded fallback password (e.g., `Dev3l0pment!2025`), but configuring it is strongly recommended.

**Important**: Do not commit real passwords or sensitive data to `appsettings.json`. Use user secrets for individual development.

## Running the Seed Process

The database seeding is part of the application startup process in the Development environment. It is configured in `Program.cs`.

To run the seed process:

1.  Ensure your database is created and migrations are applied:
    ```bash
    dotnet ef database update --project Diquis.Infrastructure
    ```
2.  Run the application:
    ```bash
    dotnet run --project Diquis.WebApi
    ```

The application will check if the database is empty and, if so, run the seeding logic automatically.

### Forcing a Re-seed

If you need to reset your database and re-run the seed process:

1.  Drop the database.
2.  Re-create the database.
3.  Run the application again. The seeder will detect the empty database and run.

```bash
# Drop the database
dotnet ef database drop --project Diquis.Infrastructure --force

# Re-apply migrations (which also creates the DB)
dotnet ef database update --project Diquis.Infrastructure

# Run the app to trigger seeding
dotnet run --project Diquis.WebApi
```

## Created Accounts

After seeding, you can log in with any of these accounts (all use the configured default password):

### Administrative
- **Super Admin**: `admin@diquis.com`
- **Academy Owner**: `owner@diquis.com`
- **Academy Admin**: `admin.academy@diquis.com`

### Staff
- **Head Coach**: `coach.main@diquis.com`
- **Fitness Staff**: `staff.fitness@diquis.com`

### Users
- **Parents**: `parent1@example.com` through `parent5@example.com`
- **Players**: `player1@example.com` through `player6@example.com`

## Security Notes

⚠️ **Important Security Practices**:

1.  **Development Only**: The seeding logic is configured to run only when the application is in the `Development` environment.
2.  **Never in Production**: The production environment should never have automatic seeding enabled.
3.  **No Hardcoded Secrets**: The seed file reads the password from configuration rather than having it hardcoded.
4.  **Strong Passwords**: The default password must meet the application's password strength requirements as configured in ASP.NET Core Identity.

## Idempotency

The seed script is designed to be idempotent. It uses `FirstOrDefault()` or `Any()` checks to avoid creating duplicate data if it's run multiple times.

## Customization

To add more seed data:

1.  Edit the seeder classes in the `Diquis.Infrastructure/Persistence/Initializer` directory.
2.  Follow the existing pattern of checking for existing data before creating new entities.
3.  Use the configured `DefaultPassword` for all user accounts.

## Troubleshooting

### Password Requirements Not Met
If you see an error during seeding related to password strength:
```
"Passwords must have at least one non-alphanumeric character."
"Passwords must have at least one digit ('0'-'9')."
```
**Solution**: Update the `DefaultPassword` in your `appsettings.Development.json` or user secrets to meet the requirements configured for ASP.NET Core Identity in `Program.cs`.

### Seeding Doesn't Run
-   Verify you are running in the `Development` environment. Check `ASPNETCORE_ENVIRONMENT` in `launchSettings.json`.
-   The seeder only runs if it detects that key tables (like Users or Academies) are empty. To force a re-seed, you must first clear the database.
