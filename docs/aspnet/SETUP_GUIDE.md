# Diquis - ASP.NET Core Setup Guide

This guide provides step-by-step instructions to set up a new project using the Diquis (ASPNano) template and configure the development environment.

## 1. Requirements

### Required Software
- **.NET SDK:** .NET 9.0 or higher is required for the template.
- **Database:**
  - SQL Server (the default configuration)
  - PostgreSQL (supported with minor configuration changes)
- **IDE/Editor:**
  - Visual Studio 2022 (recommended)
  - JetBrains Rider
  - Visual Studio Code with the C# Dev Kit extension
- **Git:** Latest version
- **.NET EF Core Tools:** `dotnet tool install --global dotnet-ef`

### Verifying Installations
Open your terminal and run `dotnet --version` to ensure the correct SDK is installed.

---

## 2. Project Scaffolding

The project is generated from the ASPNano .NET template.

### Install the Template
First, install the ASPNano template package from NuGet. This makes the `dotnet new nano` command available on your machine.
```bash
# Command to install the NuGet package (replace with actual package source if local)
# dotnet new --install Nano.Boilerplate
# Note: This is a placeholder. If you have the .nupkg file locally:
dotnet new --install /path/to/Nano.Boilerplate.nupkg
```

### Scaffold a New Project
Once the template is installed, you can create a new project.

Navigate to the directory where you want to create your solution and run:
```bash
# Format: dotnet new nano -n <ProjectName> -o <OutputDirectory> -mt <MultiTenancy> -ui <UIType>
dotnet new nano -n Diquis -o Diquis -mt multidb -ui spa
```
**Command Options:**
- `-n`: The name for your new project (e.g., `Diquis`).
- `-o`: The output directory.
- `-mt`: The multi-tenancy model.
  - `multidb`: Multi-database multi-tenancy (one DB per tenant).
  - `singledb`: Single-database multi-tenancy (shared DB with tenant identifiers).
  - `singletenant`: A standard single-tenant application.
- `-ui`: The front-end UI framework.
  - `spa`: For React or Vue.
  - `razor`: For Razor Pages (MVC).

This will create a new solution directory named `Diquis` with the complete project structure.

---

## 3. Initial Configuration

### Configure Application Secrets
The project uses the .NET Secret Manager tool to store sensitive configuration.

Navigate to the `Diquis.WebApi` project directory:
```bash
cd Diquis/Diquis.WebApi
```

Initialize user secrets:
```bash
dotnet user-secrets init
```

Set the connection string for the **shared tenant database**:
```bash
# For SQL Server (default)
dotnet user-secrets set "ConnectionStrings:TenantConnection" "Server=.;Database=DiquisTenants;Trusted_Connection=True;TrustServerCertificate=True;"
```
*This connection string points to the central database that holds the list of all tenants.*

### Review `appsettings.json`
Open the `appsettings.json` file in the `Diquis.WebApi` project. Here you can configure:
- Logging levels
- JWT settings (though the secret key should be in user secrets)
- Default connection strings for different environments

---

## 4. Database Setup

### Create the Shared Database
Ensure your SQL Server instance is running. You need to create an empty database for the tenant metadata.
```sql
CREATE DATABASE DiquisTenants;
```

### Apply Initial Migrations
The database schema is managed by EF Core Migrations. To create the tables in the shared database, run the following command from the root of the repository:
```bash
dotnet ef database update --context TenantDbContext --project Diquis.Infrastructure
```
This command specifically targets the `TenantDbContext` for the shared database.

---

## 5. Running the Application

### Restore Dependencies
Navigate to the root of the repository and restore all NuGet packages:
```bash
dotnet restore
```

### Run the API
```bash
dotnet run --project Diquis.WebApi
```
The API will start and listen on the ports configured in `Diquis.WebApi/Properties/launchSettings.json` (typically `https://localhost:5001`).

### Default Admin User
The database is seeded with a default administrator user:
- **Email:** `admin@admin.com`
- **Password:** `admin`

You can use these credentials to log in and receive a JWT token.

### Tenant Databases
When a new tenant is created and accessed for the first time, the application will automatically:
1. Create a new database for that tenant.
2. Apply the `ApplicationDbContext` migrations to the new database.

This process is handled by the application's multi-tenancy logic.
