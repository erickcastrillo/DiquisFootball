# Task Context
Create a validation script to perform smoke tests on newly provisioned tenants. This script ensures that the infrastructure is correctly configured and the application is reachable for the specific tenant.

# Core References
- **Plan:** [1 - Implementation_Plan_Infrastructure.md](./1%20-%20Implementation_Plan_Infrastructure.md)
- **Tech Guide:** [CommercialOnboarding_TechnicalGuide.md](../../Technical%20documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md)
- **FRS:** [CommercialOnboarding_FRS.md](../../Business%20Requirements/CommercialOnboarding_FRS/CommercialOnboarding_FRS.md)

# Step-by-Step Instructions
1.  **Create Script:** Create `scripts/tenant-smoke-test.ps1`.
2.  **Parameters:** Accept `TenantIdentifier` and `BaseUrl`.
3.  **Construct URL:** Build the health check URL (e.g., `https://api.$BaseUrl/health` or `https://$TenantIdentifier.$BaseUrl/health`).
4.  **Execute Request:** Use `Invoke-RestMethod` to send a GET request.
    *   Include `X-Tenant-ID` header if required by your routing strategy.
5.  **Validate:** Check for HTTP 200 OK status.
6.  **Output:** Return exit code 0 for success, 1 for failure.

# Acceptance Criteria
- [ ] File `scripts/tenant-smoke-test.ps1` exists.
- [ ] Script accepts tenant details.
- [ ] Script performs HTTP request with correct headers/URL.
- [ ] Script returns appropriate exit codes.
