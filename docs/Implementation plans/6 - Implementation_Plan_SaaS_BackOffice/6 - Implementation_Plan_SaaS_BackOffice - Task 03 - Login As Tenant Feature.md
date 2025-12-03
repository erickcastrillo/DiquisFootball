# Task Context
Implement the "Login As Tenant" (Impersonation) feature. This allows Super Admins to generate a temporary JWT for a specific tenant's owner to debug issues. The feature requires a secure backend endpoint and a frontend action button.

# Core References
- **Plan:** [6 - Implementation_Plan_SaaS_BackOffice.md](./6%20-%20Implementation_Plan_SaaS_BackOffice.md)

# Step-by-Step Instructions
1.  **Backend Endpoint:**
    *   Controller: `IdentityController` (or similar).
    *   Endpoint: `POST /api/identity/impersonate/{tenantId}`.
    *   Security: `[Authorize(Roles = "SystemSuperAdmin")]`.
    *   Logic:
        *   Find primary owner of tenant.
        *   Generate new JWT with extra claims: `impersonator_id`, `original_token`.
        *   Return token.
2.  **Frontend Action:**
    *   In `TenantList.tsx` (or similar admin table).
    *   Add "Login As" button.
    *   On click:
        *   Call API.
        *   Save new token to localStorage.
        *   Redirect to dashboard (`window.location.href = '/dashboard'`).

# Acceptance Criteria
- [ ] Secure API endpoint exists for token generation.
- [ ] Impersonation token contains tracking claims.
- [ ] Frontend button successfully logs the admin in as the tenant.
