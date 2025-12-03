# 1 - Implementation_Plan_Infrastructure - Detailed Tasks

This document details the atomic tasks required to implement the DevOps and Hybrid Infrastructure plan for the Diquis SaaS Platform.

## Task List

### Task 01: Infrastructure as Code - Shared Infrastructure Module
**Description:** Create the Terraform module that provisions the core shared infrastructure used by Grassroots and Professional tenants. This includes the main App Service, the shared PostgreSQL server, and the Container Registry.
**Relevant Files:**
*   `docs/Implementation plans/1 - Implementation_Plan_Infrastructure/1 - Implementation_Plan_Infrastructure.md`
*   `docs/Technical documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md`
*   `README.md`
**Step-by-Step Instructions:**
1.  Create a new directory: `infrastructure/terraform/modules/shared_infra`.
2.  Create `main.tf` in this directory.
3.  Define an Azure Resource Group resource (`azurerm_resource_group`) named `rg-diquis-shared-prod`.
4.  Define an Azure App Service Plan (`azurerm_service_plan`) with `os_type = "Linux"` and `sku_name = "P2v3"` (or appropriate tier).
5.  Define an Azure Linux Web App (`azurerm_linux_web_app`) linked to the service plan. Configure `site_config` to use a Docker container from the ACR (e.g., `diquisacr.azurecr.io/api:latest`).
6.  Define an Azure Database for PostgreSQL Flexible Server (`azurerm_postgresql_flexible_server`) to serve as the shared database instance.
7.  Define an Azure Container Registry (`azurerm_container_registry`) to store application images.
8.  Create `variables.tf` to allow customization (e.g., location, resource names) and `outputs.tf` to export critical IDs (e.g., ACR login server, App Service default hostname).
**Technical Considerations:**
*   Use standard Azure naming conventions.
*   Ensure the Web App has `SystemAssigned` identity enabled for future KeyVault access.
*   Configure the PostgreSQL server with appropriate extensions if needed (e.g., `uuid-ossp`).
**Verification (Definition of Done):**
*   [ ] Directory `infrastructure/terraform/modules/shared_infra` exists.
*   [ ] `terraform validate` runs successfully in the module directory.
*   [ ] `main.tf` contains definitions for RG, App Service Plan, Web App, PostgreSQL, and ACR.

### Task 02: Infrastructure as Code - Professional Tenant Module
**Description:** Create the Terraform module for provisioning "Professional" tier tenants. These tenants share the application compute but require a dedicated database for isolation.
**Relevant Files:**
*   `docs/Implementation plans/1 - Implementation_Plan_Infrastructure/1 - Implementation_Plan_Infrastructure.md`
*   `docs/Technical documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md`
**Step-by-Step Instructions:**
1.  Create a new directory: `infrastructure/terraform/modules/professional_tenant`.
2.  Create `main.tf`, `variables.tf`, and `outputs.tf`.
3.  In `variables.tf`, define inputs: `tenant_name`, `resource_group_name`, `location`, `administrator_login`, `administrator_password`.
4.  In `main.tf`, define an `azurerm_postgresql_flexible_server` resource. The name should be dynamic based on `var.tenant_name` (e.g., `psql-diquis-${var.tenant_name}`).
5.  Define an `azurerm_postgresql_database` resource named `db_application` associated with the new server.
6.  In `outputs.tf`, export the `connection_string` (marked as sensitive).
**Technical Considerations:**
*   Ensure the module is reusable for multiple tenants.
*   The database charset should be `UTF8`.
**Verification (Definition of Done):**
*   [ ] Directory `infrastructure/terraform/modules/professional_tenant` exists.
*   [ ] `terraform validate` runs successfully.
*   [ ] Module accepts `tenant_name` as input and generates a unique server name.

### Task 03: Infrastructure as Code - Enterprise Tenant Module
**Description:** Create the Terraform module for "Enterprise" tier tenants. These tenants require a fully isolated stack (Resource Group, App Service, etc.).
**Relevant Files:**
*   `docs/Implementation plans/1 - Implementation_Plan_Infrastructure/1 - Implementation_Plan_Infrastructure.md`
*   `docs/Technical documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md`
**Step-by-Step Instructions:**
1.  Create a new directory: `infrastructure/terraform/modules/enterprise_tenant`.
2.  Create `main.tf`, `variables.tf`, and `outputs.tf`.
3.  In `variables.tf`, define `tenant_name` and other necessary config.
4.  In `main.tf`, define a new `azurerm_resource_group` specific to the tenant.
5.  Define a new `azurerm_service_plan` and `azurerm_linux_web_app` within this new RG.
6.  (Optional) Call the `professional_tenant` module (or similar logic) to provision the dedicated database within this new RG.
**Technical Considerations:**
*   This module represents a "Stamp" architecture.
*   Ensure resource names are unique globally where required (e.g., App Service default hostnames).
**Verification (Definition of Done):**
*   [ ] Directory `infrastructure/terraform/modules/enterprise_tenant` exists.
*   [ ] `terraform validate` runs successfully.
*   [ ] Module creates a complete, isolated environment.

### Task 04: CI/CD - Build and Containerize Pipeline
**Description:** Implement the GitHub Actions workflow to build the .NET API and React Client, run tests, and push Docker images to the Azure Container Registry.
**Relevant Files:**
*   `docs/Implementation plans/1 - Implementation_Plan_Infrastructure/1 - Implementation_Plan_Infrastructure.md`
*   `README.md` (for build commands)
**Step-by-Step Instructions:**
1.  Create `.github/workflows/build.yml`.
2.  Define triggers: `push` to `main` and `workflow_dispatch`.
3.  Define a job `build_api`:
    *   Use `ubuntu-latest`.
    *   Checkout code.
    *   Setup .NET SDK (v10.0.x).
    *   Run `dotnet restore Diquis.sln`.
    *   Run `dotnet build Diquis.sln -c Release`.
    *   Run `dotnet test Diquis.sln --no-build`.
    *   Log in to ACR using `azure/docker-login` (use secrets `ACR_NAME`, `ACR_USERNAME`, `ACR_PASSWORD`).
    *   Run `docker build` for the API project (`Diquis.WebApi/Dockerfile`).
    *   Run `docker push` tagging with `${{ github.sha }}` and `latest`.
**Technical Considerations:**
*   Ensure secrets are referenced correctly (`${{ secrets.ACR_NAME }}`).
*   Optimize Docker build with layer caching if possible.
**Verification (Definition of Done):**
*   [ ] File `.github/workflows/build.yml` exists.
*   [ ] Workflow contains steps for Restore, Build, Test, and Docker Push.

### Task 05: CI/CD - Release and Deployment Pipeline
**Description:** Implement the GitHub Actions workflow for deploying the application. It should handle the shared environment (staging/production slots) and enterprise environments (matrix deployment).
**Relevant Files:**
*   `docs/Implementation plans/1 - Implementation_Plan_Infrastructure/1 - Implementation_Plan_Infrastructure.md`
**Step-by-Step Instructions:**
1.  Create `.github/workflows/release.yml`.
2.  Define `workflow_dispatch` trigger with input `image_tag`.
3.  Define job `deploy_shared`:
    *   Use `azure/webapps-deploy` to deploy the image to the `staging` slot of the shared App Service.
    *   Include a step (or separate job) to swap slots to production.
4.  Define job `deploy_enterprise` (needs `deploy_shared`):
    *   Use a `matrix` strategy. (Note: For this task, you can hardcode a sample matrix or comment on how to fetch it dynamically).
    *   Iterate through tenants and deploy to their specific App Services using `azure/webapps-deploy`.
**Technical Considerations:**
*   Use `azure/login` action for authentication with Azure.
*   Ensure the `image_tag` input is passed correctly to the deployment action.
**Verification (Definition of Done):**
*   [ ] File `.github/workflows/release.yml` exists.
*   [ ] Workflow defines jobs for Shared and Enterprise deployments.

### Task 06: Database Migration Script
**Description:** Develop a PowerShell script to execute Entity Framework Core migrations across all tenant databases in parallel.
**Relevant Files:**
*   `docs/Implementation plans/1 - Implementation_Plan_Infrastructure/1 - Implementation_Plan_Infrastructure.md`
**Step-by-Step Instructions:**
1.  Create directory `scripts` if it doesn't exist.
2.  Create `scripts/run-migrations.ps1`.
3.  Define parameters: `$ManagementDbConnectionString`, `$MaxParallelJobs`.
4.  Implement logic to connect to the Management DB and select all tenant connection strings.
5.  Use `ForEach-Object -Parallel` to iterate through connection strings.
6.  Inside the loop, run `dotnet ef database update --connection $ConnectionString`.
7.  Add error handling (`try/catch`) and logging for failed migrations.
**Technical Considerations:**
*   Requires `dotnet-ef` tool to be installed on the runner.
*   Ensure connection strings are handled securely.
**Verification (Definition of Done):**
*   [ ] File `scripts/run-migrations.ps1` exists.
*   [ ] Script includes parallel execution logic.
*   [ ] Script queries the database for tenant info.

### Task 07: Tenant Smoke Test Script
**Description:** Create a script to verify the health of a newly provisioned tenant by hitting its API endpoint.
**Relevant Files:**
*   `docs/Implementation plans/1 - Implementation_Plan_Infrastructure/1 - Implementation_Plan_Infrastructure.md`
**Step-by-Step Instructions:**
1.  Create `scripts/tenant-smoke-test.ps1`.
2.  Define parameters: `$TenantIdentifier`, `$BaseUrl`.
3.  Construct the API URL (e.g., `https://api.$BaseUrl/health`).
4.  Use `Invoke-RestMethod` to send a GET request. Include the `X-Tenant-ID` header if using header-based routing, or rely on the subdomain if using host-based routing.
5.  Check if the response status code is 200.
6.  Exit with 0 for success, 1 for failure.
**Technical Considerations:**
*   This script will be used by the Provisioning Service or CI/CD pipeline.
**Verification (Definition of Done):**
*   [ ] File `scripts/tenant-smoke-test.ps1` exists.
*   [ ] Script makes an HTTP request with appropriate headers/URL.
*   [ ] Script validates the response code.
