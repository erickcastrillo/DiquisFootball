# Task Context
Initialize the Infrastructure as Code (IaC) foundation by creating the shared infrastructure module. This module will provision the core resources used by Grassroots and Professional tenants, including the App Service Plan, Web App, and Shared PostgreSQL Server.

# Core References
- **Plan:** [1 - Implementation_Plan_Infrastructure.md](./1%20-%20Implementation_Plan_Infrastructure.md)
- **Tech Guide:** [CommercialOnboarding_TechnicalGuide.md](../../Technical%20documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md)
- **FRS:** [CommercialOnboarding_FRS.md](../../Business%20Requirements/CommercialOnboarding_FRS/CommercialOnboarding_FRS.md)

# Step-by-Step Instructions
1.  **Create Module Directory:** Create a directory `modules/shared_infra` in your infrastructure root (e.g., `infra/terraform`).
2.  **Define Resources (`main.tf`):**
    *   **Resource Group:** `azurerm_resource_group` named `rg-diquis-shared-prod`.
    *   **App Service Plan:** `azurerm_service_plan` (Linux, P2v3 or similar production SKU).
    *   **Web App:** `azurerm_linux_web_app` linked to the plan. Configure `site_config` to use the Docker image `diquisacr.azurecr.io/api:latest`.
    *   **Database:** `azurerm_postgresql_flexible_server` for the shared database instance.
    *   **Registry:** `azurerm_container_registry` (ACR) for storing images.
3.  **Define Variables & Outputs:**
    *   Create `variables.tf` for location, tags, etc.
    *   Create `outputs.tf` to expose the ACR Login Server, App Service Default Hostname, and Database FQDN.

# Acceptance Criteria
- [ ] Directory `modules/shared_infra` exists.
- [ ] `main.tf` contains valid Terraform configuration for RG, App Service Plan, Web App, Postgres, and ACR.
- [ ] `terraform validate` runs successfully in the module directory.
