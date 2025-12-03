# Task Context
Create the Terraform module for "Enterprise" tier tenants. These tenants require a fully isolated stack, including their own Resource Group, App Service Plan, and Web App, ensuring maximum security and performance isolation.

# Core References
- **Plan:** [1 - Implementation_Plan_Infrastructure.md](./1%20-%20Implementation_Plan_Infrastructure.md)
- **Tech Guide:** [CommercialOnboarding_TechnicalGuide.md](../../Technical%20documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md)
- **FRS:** [CommercialOnboarding_FRS.md](../../Business%20Requirements/CommercialOnboarding_FRS/CommercialOnboarding_FRS.md)

# Step-by-Step Instructions
1.  **Create Module Directory:** Create `modules/enterprise_tenant`.
2.  **Define Inputs (`variables.tf`):**
    *   `tenant_name` (string)
    *   `location` (string)
3.  **Define Resources (`main.tf`):**
    *   **Resource Group:** `azurerm_resource_group` named `rg-diquis-${var.tenant_name}-prod`.
    *   **App Service Plan:** `azurerm_service_plan` within the new RG.
    *   **Web App:** `azurerm_linux_web_app` named `app-diquis-${var.tenant_name}-api`.
    *   **Database:** Reuse the logic from Task 02 (or call the module) to provision a dedicated database in this RG.
4.  **Define Outputs (`outputs.tf`):**
    *   `app_url`: The URL of the isolated web app.
    *   `db_connection_string`.

# Acceptance Criteria
- [ ] Directory `modules/enterprise_tenant` exists.
- [ ] Module creates a fully isolated environment (RG, Plan, App, DB).
- [ ] Resources are named dynamically based on `tenant_name`.
- [ ] `terraform validate` runs successfully.
