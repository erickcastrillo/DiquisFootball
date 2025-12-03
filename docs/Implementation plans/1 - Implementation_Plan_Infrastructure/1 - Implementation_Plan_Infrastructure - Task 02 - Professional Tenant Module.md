# Task Context
Create the Terraform module responsible for provisioning resources for "Professional" tier tenants. These tenants share the application runtime but require a dedicated database for data isolation.

# Core References
- **Plan:** [1 - Implementation_Plan_Infrastructure.md](./1%20-%20Implementation_Plan_Infrastructure.md)
- **Tech Guide:** [CommercialOnboarding_TechnicalGuide.md](../../Technical%20documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md)
- **FRS:** [CommercialOnboarding_FRS.md](../../Business%20Requirements/CommercialOnboarding_FRS/CommercialOnboarding_FRS.md)

# Step-by-Step Instructions
1.  **Create Module Directory:** Create `modules/professional_tenant`.
2.  **Define Inputs (`variables.tf`):**
    *   `tenant_name` (string)
    *   `resource_group_name` (string)
    *   `location` (string)
    *   `admin_login` / `admin_password` (secure strings)
3.  **Define Resources (`main.tf`):**
    *   **PostgreSQL Server:** `azurerm_postgresql_flexible_server`. Name must be dynamic: `psql-diquis-${var.tenant_name}`.
    *   **Database:** `azurerm_postgresql_database` named `db_application` inside the server.
4.  **Define Outputs (`outputs.tf`):**
    *   `db_connection_string`: Construct and output the connection string (mark as `sensitive = true`).

# Acceptance Criteria
- [ ] Directory `modules/professional_tenant` exists.
- [ ] Module accepts `tenant_name` and creates a uniquely named PostgreSQL server.
- [ ] Module outputs the connection string.
- [ ] `terraform validate` runs successfully.
