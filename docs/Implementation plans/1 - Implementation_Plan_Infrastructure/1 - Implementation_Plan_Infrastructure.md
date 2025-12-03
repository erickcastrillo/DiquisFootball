# DevOps & Hybrid Infrastructure: Implementation and Testing Plan

## 1. Executive Summary

This document outlines the comprehensive DevOps strategy for the Diquis SaaS Platform, covering Infrastructure as Code (IaC), CI/CD pipelines, large-scale database management, and post-provisioning testing. The plan is designed to support our hybrid tenancy model, ensuring automated, scalable, and reliable delivery for all client tiers.

## 2. Infrastructure as Code (IaC)

We will use **Terraform** to define and manage our cloud infrastructure, enabling consistent and repeatable environments. The structure will be modular to cater to our different commercial tiers.

### 2.1. Core Module: Shared Infrastructure (`modules/shared_infra`)

This module will provision the resources shared by **Grassroots** and **Professional** tenants.

*   **Azure App Service Plan:** A single, robust plan to host the core web application.
*   **Azure App Service:** The main web application, configured for container deployment.
*   **Azure Database for PostgreSQL (Flexible Server):** A single server instance to host the `BaseDbContext` (management DB) and all shared `ApplicationDbContext` databases for Grassroots tenants.
*   **Azure Container Registry:** To store Docker images for the API and client-side application.
*   **Networking & Security:** VNet, subnets, and Network Security Groups (NSGs).

```terraform
# modules/shared_infra/main.tf

resource "azurerm_resource_group" "core" {
  name     = "rg-diquis-shared-prod"
  location = "East US 2"
}

resource "azurerm_service_plan" "main" {
  name                = "asp-diquis-prod"
  resource_group_name = azurerm_resource_group.core.name
  location            = azurerm_resource_group.core.location
  os_type             = "Linux"
  sku_name            = "P2v3" # Example SKU
}

resource "azurerm_linux_web_app" "main" {
  name                = "app-diquis-api-prod"
  resource_group_name = azurerm_resource_group.core.name
  location            = azurerm_service_plan.main.location
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    application_stack {
      docker_image     = "diquisacr.azurecr.io/api"
      docker_image_tag = "latest"
    }
  }
}

resource "azurerm_postgresql_flexible_server" "shared" {
  name                = "psql-diquis-shared-prod"
  resource_group_name = azurerm_resource_group.core.name
  location            = azurerm_resource_group.core.location
  # ... other configuration for the shared server
}
```

### 2.2. Tenant-Specific Modules

#### 2.2.1. Professional Tier (`modules/professional_tenant`)

This module is invoked by the `ProvisioningService` for each **Professional** tenant. It provisions a dedicated database.

```terraform
# modules/professional_tenant/main.tf

variable "tenant_name" { type = string }
variable "resource_group_name" { type = string }
variable "location" { type = string }

resource "azurerm_postgresql_flexible_server" "dedicated" {
  name                = "psql-diquis-${var.tenant_name}-prod"
  resource_group_name = var.resource_group_name
  location            = var.location
  # ... configuration for a dedicated server
}

resource "azurerm_postgresql_database" "app_db" {
  name                = "db_application"
  resource_group_name = var.resource_group_name
  server_name         = azurerm_postgresql_flexible_server.dedicated.name
  charset             = "UTF8"
  collation           = "en_US.utf8"
}

output "db_connection_string" {
  value     = azurerm_postgresql_flexible_server.dedicated.connection_string # Simplified for example
  sensitive = true
}
```

#### 2.2.2. Enterprise Tier (`modules/enterprise_tenant`)

This module provisions a fully isolated stack for an **Enterprise** tenant.

```terraform
# modules/enterprise_tenant/main.tf

variable "tenant_name" { type = string }

resource "azurerm_resource_group" "tenant_rg" {
  name     = "rg-diquis-${var.tenant_name}-prod"
  location = "East US 2"
}

resource "azurerm_service_plan" "tenant_asp" {
  name                = "asp-diquis-${var.tenant_name}"
  resource_group_name = azurerm_resource_group.tenant_rg.name
  # ...
}

resource "azurerm_linux_web_app" "tenant_app" {
  name                = "app-diquis-${var.tenant_name}-api"
  resource_group_name = azurerm_resource_group.tenant_rg.name
  service_plan_id     = azurerm_service_plan.tenant_asp.id
  # ...
}

# The dedicated database would be provisioned using the `professional_tenant` module
# or a similar dedicated database module.
```

## 3. CI/CD Pipelines (GitHub Actions)

### 3.1. Build & Containerization Pipeline

This pipeline triggers on pushes to the `main` branch. It builds, tests, and containerizes the .NET API and the React client, pushing the images to Azure Container Registry.

**`.github/workflows/build.yml`**
```yaml
name: Build and Containerize

on:
  push:
    branches: [ main ]
  workflow_dispatch:

jobs:
  build_and_push_api:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3

    - name: Set up .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'

    - name: Restore dependencies
      run: dotnet restore Diquis.sln

    - name: Build
      run: dotnet build Diquis.sln --no-restore --configuration Release

    - name: Test
      run: dotnet test Diquis.sln --no-build --verbosity normal

    - name: Log in to Azure Container Registry
      uses: azure/docker-login@v1
      with:
        login-server: ${{ secrets.ACR_NAME }}.azurecr.io
        username: ${{ secrets.ACR_USERNAME }}
        password: ${{ secrets.ACR_PASSWORD }}

    - name: Build and push API Docker image
      run: |
        docker build . -f Diquis.WebApi/Dockerfile -t ${{ secrets.ACR_NAME }}.azurecr.io/api:${{ github.sha }} -t ${{ secrets.ACR_NAME }}.azurecr.io/api:latest
        docker push ${{ secrets.ACR_NAME }}.azurecr.io/api --all-tags

  # A similar job would exist for the React client (build_and_push_client)
```

### 3.2. Release & Deployment Pipeline

This pipeline handles deployments. It's designed to update all tenant environments in a controlled manner.

**Strategy:**
1.  **Staging Deployment:** The pipeline first deploys to a dedicated staging environment that mimics the shared infrastructure.
2.  **Shared Environment Release:** Upon approval, it deploys the new container image to the shared App Service, updating all **Grassroots** and **Professional** tenants simultaneously using a blue-green slot swap.
3.  **Enterprise Release (Matrix Deployment):** The pipeline then queries the management API (or `BaseDbContext`) to get a list of all Enterprise tenants and their resource group/app service names. It uses a GitHub Actions matrix strategy to deploy to each isolated environment in parallel.

**`.github/workflows/release.yml`** (Conceptual)
```yaml
name: Release and Deploy

on:
  workflow_dispatch:
    inputs:
      image_tag:
        description: 'Image tag to deploy (e.g., commit SHA)'
        required: true

jobs:
  deploy_shared:
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to Shared App Service Staging Slot
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'app-diquis-api-prod'
          slot-name: 'staging'
          images: '${{ secrets.ACR_NAME }}.azurecr.io/api:${{ github.event.inputs.image_tag }}'
          
      # Manual approval step would go here
      
      - name: Swap Staging to Production for Shared App
        # ... azure cli script to swap slots

  deploy_enterprise:
    needs: deploy_shared
    runs-on: ubuntu-latest
    strategy:
      matrix:
        tenant: ${{ fromJson(needs.get_tenants.outputs.matrix) }} # Output from a previous job that gets tenant list
    steps:
      - name: Deploy to Enterprise Tenant ${{ matrix.tenant.name }}
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'app-diquis-${{ matrix.tenant.name }}-api'
          resource-group-name: 'rg-diquis-${{ matrix.tenant.name }}-prod'
          images: '${{ secrets.ACR_NAME }}.azurecr.io/api:${{ github.event.inputs.image_tag }}'
```

## 4. Database Migration Management

To manage schema updates across hundreds or thousands of tenant databases, a parallelized script is essential.

### 4.1. `run-migrations.ps1` (PowerShell)

This script will be executed from a DevOps agent or a management VM that has `dotnet-ef` tools and network access to the databases.

```powershell
param(
    [string]$ManagementDbConnectionString,
    [int]$MaxParallelJobs = 10
)

# Install required modules if not present
# Install-Module -Name SqlServer -Scope CurrentUser -Force

Write-Host "Fetching list of tenant databases..."

# Query the management database to get connection strings for all dedicated DBs
$query = "SELECT ""ConnectionString"" FROM ""Tenants"" WHERE ""SubscriptionTier"" IN ('Professional', 'Enterprise') AND ""ConnectionString"" IS NOT NULL;"
$connectionStrings = Invoke-Sqlcmd -ConnectionString $ManagementDbConnectionString -Query $query | Select-Object -ExpandProperty ConnectionString

$jobScript = {
    param($connString, $efProjectPath)
    
    Write-Host "Starting migration for a tenant..."
    try {
        # Execute the EF Core migration command
        dotnet ef database update --project $efProjectPath --connection $connString
        Write-Host "Migration SUCCEEDED for a tenant."
    } catch {
        Write-Error "Migration FAILED for a tenant. Error: $_"
        # Log the failed connection string for manual review
        $connString | Out-File -FilePath "./failed_migrations.log" -Append
    }
}

$efProjectPath = "../Diquis.Infrastructure/Diquis.Infrastructure.csproj" # Relative path to infra project

# Run jobs in parallel
$connectionStrings | ForEach-Object -ThrottleLimit $MaxParallelJobs -Parallel {
    & $using:jobScript -connString $_ -efProjectPath $using:efProjectPath
}

Write-Host "All migration jobs complete."

```

## 5. Testing and Validation Strategy

### 5.1. `tenant-smoke-test.ps1`

After the `ProvisioningService` completes the creation of a new tenant's infrastructure, this script will be triggered to perform a basic health check.

```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$TenantIdentifier, # e.g., the tenant's unique subdomain part or an ID
    
    [Parameter(Mandatory=$true)]
    [string]$BaseUrl # e.g., 'diquis.com'
)

# Determine the tenant's full URL
# This logic depends on the final multi-tenancy routing strategy (header vs. subdomain)
# Example using a header:
$apiUrl = "https://api.$BaseUrl/api/health" # A public health-check endpoint
$headers = @{
    "X-Tenant-ID" = $TenantIdentifier
}

Write-Host "Performing smoke test for tenant '$TenantIdentifier' at '$apiUrl'"

try {
    $response = Invoke-RestMethod -Uri $apiUrl -Method Get -Headers $headers -TimeoutSec 30
    
    # Assuming a 200-OK response is a success
    Write-Host "Smoke Test PASSED for tenant '$TenantIdentifier'. Endpoint returned a successful status."
    exit 0 # Success
}
catch {
    Write-Error "Smoke Test FAILED for tenant '$TenantIdentifier'. Details: $_"
    exit 1 # Failure
}
```
This script provides a simple, automated way to confirm that networking, DNS, and the application service are correctly configured and reachable for a newly provisioned tenant.
