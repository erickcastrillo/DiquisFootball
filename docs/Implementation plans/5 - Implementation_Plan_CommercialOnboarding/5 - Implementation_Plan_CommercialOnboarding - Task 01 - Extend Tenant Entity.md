# Task Context
Extend the `Tenant` entity in the Domain layer to support commercial features. This involves adding properties for subscription tier, status, Stripe customer ID, and a read-only flag. This is foundational for managing the tenant's commercial lifecycle.

# Core References
- **Plan:** [5 - Implementation_Plan_CommercialOnboarding.md](./5%20-%20Implementation_Plan_CommercialOnboarding.md)
- **Tech Guide:** [CommercialOnboarding_TechnicalGuide.md](../../Technical%20documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Modify `Tenant.cs`:**
    *   Path: `Diquis.Domain/Entities/Multitenancy/Tenant.cs`
    *   Add `SubscriptionTier` (Enum).
    *   Add `SubscriptionStatus` (Enum).
    *   Add `StripeCustomerId` (string?).
    *   Add `IsReadOnly` (bool).
2.  **Define `SubscriptionStatus` Enum:**
    *   If not already present, define it in `Diquis.Domain/Enums/SubscriptionStatus.cs` or within the same file if appropriate (prefer separate file).
    *   Values: `Active`, `Trial`, `Provisioning`, `ProvisioningFailed`, `PastDue`, `Canceled`.
3.  **Create Migration:**
    *   (Note for Agent: You won't run the migration command here, but the code must be ready for it).
    *   Ensure `BaseDbContext` includes these new properties in the `Tenants` DbSet.

# Acceptance Criteria
- [ ] `Tenant.cs` includes the new properties.
- [ ] `SubscriptionStatus` enum is defined.
- [ ] Solution builds successfully.
