# Task Context
Implement the backend logic to handle Stripe webhooks, specifically `checkout.session.completed`. This involves creating a controller to receive the webhook and a service to process the successful payment, create the tenant record, and trigger the provisioning job.

# Core References
- **Plan:** [5 - Implementation_Plan_CommercialOnboarding.md](./5%20-%20Implementation_Plan_CommercialOnboarding.md)
- **Tech Guide:** [CommercialOnboarding_TechnicalGuide.md](../../Technical%20documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `StripeWebhookController`:**
    *   Path: `Diquis.WebApi/Controllers/StripeWebhookController.cs`
    *   Route: `api/webhooks/stripe`
    *   Method: `POST`
    *   Logic: Read body, validate signature using `EventUtility.ConstructEvent`, check for `checkout.session.completed`, call `IOnboardingService`.
2.  **Create/Update `IOnboardingService`:**
    *   Add method `HandleSuccessfulSignUpAsync(Session session)`.
3.  **Implement `OnboardingService` Logic:**
    *   Extract metadata (TenantName, Tier) from session.
    *   Create `Tenant` entity with status `Provisioning`.
    *   Create `ApplicationUser` (Owner).
    *   Save to `BaseDbContext`.
    *   Enqueue job: `_backgroundJobService.Enqueue<IProvisioningService>(s => s.ProvisionTenantAsync(tenant.Id))`.

# Acceptance Criteria
- [ ] `StripeWebhookController` exists and validates signatures.
- [ ] `OnboardingService` handles the creation of Tenant and User.
- [ ] Background job is enqueued upon success.
