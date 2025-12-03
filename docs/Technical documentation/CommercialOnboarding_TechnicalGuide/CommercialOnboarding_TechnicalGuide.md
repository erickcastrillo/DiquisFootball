# Technical Implementation Guide: Commercial Onboarding & Provisioning

This document provides a detailed technical guide for implementing the "Commercial Onboarding & Provisioning" module. This module is the cornerstone of the Diquis SaaS business model, involving payment gateway integration, automated infrastructure provisioning, and complex background job orchestration.

## 1. Architectural Analysis

### Domain Entities

This module requires extending the existing `Tenant` entity and introducing a new `Contract` entity.

1.  **`Tenant`** (Extend existing entity in `Diquis.Domain`):
    *   Add `SubscriptionTier` (enum: `Grassroots`, `Professional`, `Enterprise`).
    *   Add `StripeCustomerId` (string, nullable): Stores the customer ID from Stripe for billing.
    *   Add `SubscriptionStatus` (enum: `Active`, `Trial`, `PastDue`, `Canceled`, `ProvisioningFailed`).
    *   Add `ContractId` (Guid, nullable, FK to `Contract`).
    *   Add `IsReadOnly` (bool): A maintenance flag to be used during upgrades.

2.  **`Contract`** (New entity, in `Diquis.Domain`): Represents a signed enterprise agreement.
    *   `TenantId` (Guid, FK to `Tenant`).
    *   `OriginalFilename` (string).
    *   `StorageUrl` (string): Secure URL to the stored contract PDF (e.g., in Azure Blob Storage).
    *   `UploadedBy` (Guid, FK to `ApplicationUser`).
    *   `ContractEndDate` (DateTime).

### Multi-Tenancy Scope

-   **`BaseDbContext` (Management DB):**
    -   The `Tenant` entity and the new `Contract` entity reside here. This data is part of the system's control plane, not the application data for a specific tenant.
-   **Provisioning Logic:** The services and jobs for this module operate at a system level, outside the context of a single tenant's `ApplicationDbContext`.

### Permissions & Authorization

-   **`Prospective Client` (Anonymous):** Public-facing endpoints for self-service sign-up must be decorated with `[AllowAnonymous]`.
-   **`System Super-Admin` (`super_user`):** This role will have access to the manual enterprise provisioning endpoints. A new policy `IsSuperAdmin` should be created.
-   **`Academy Owner`**: This role will have access to the subscription management/upgrade endpoints for their own tenant.

## 2. Scaffolding Strategy (CLI)

Standard CRUD scaffolding is **not appropriate** for this orchestration-heavy module. The implementation will involve extending existing services and creating new, specialized services manually.

1.  **Extend `TenantManagementService`**: Add methods for enterprise client creation and subscription upgrades.
2.  **Manual Service Creation**:
    *   **`IOnboardingService` / `OnboardingService.cs`**: In `Diquis.Application`, to handle the self-service sign-up flow logic.
    *   **`IProvisioningService` / `ProvisioningService.cs`**: In `Diquis.Application`, to orchestrate the infrastructure creation.
    *   **`IStripeService` / `StripeService.cs`**: In `Diquis.Infrastructure`, to encapsulate all interactions with the Stripe API.
3.  **Manual Controller Creation**:
    *   `OnboardingController.cs`: For public, anonymous-access endpoints related to self-service sign-up.
    *   `StripeWebhookController.cs`: A dedicated, anonymous-access controller to receive webhooks from Stripe.
    *   `SubscriptionController.cs`: For authenticated `Academy Owner`s to manage their plans.

## 3. Implementation Plan (Agile Breakdown)

### User Story: Public Self-Service Onboarding
**As a** Prospective Client, **I want to** select a plan, pay, and get immediate access, **so that** I can start using the platform quickly.

**Technical Tasks:**
1.  **Domain:** Add the new properties to the `Tenant` entity.
2.  **Persistence:** Create a new migration for the `BaseDbContext` to update the `Tenants` table.
    -   *Migration Command:* `add-migration -Context BaseDbContext -o Persistence/Migrations/BaseDb AddSubscriptionFieldsToTenant`
3.  **Application (DTOs):** Create `SelfServiceSignUpRequest` (Email, Password, AcademyName, Tier).
4.  **Application (Service):** In `OnboardingService.cs`:
    -   Create `InitiateSignUpAsync(SelfServiceSignUpRequest request)`. This method creates a `Stripe.Checkout.Session` via the `IStripeService` and returns the session URL to the client.
    -   Create `HandleSuccessfulPaymentWebhookAsync(Stripe.Event stripeEvent)`. This is the core logic. It will:
        -   Extract customer and subscription details from the Stripe event.
        -   Create the `ApplicationUser` and the `Tenant` record, setting its status to "Provisioning".
        -   Enqueue a background job via `IBackgroundJobService.Enqueue<IProvisioningService>(s => s.ProvisionTenantAsync(tenant.Id))`.
5.  **API:**
    -   In `OnboardingController.cs`, create `POST /api/onboarding/signup`. It will be `[AllowAnonymous]` and will call `OnboardingService.InitiateSignUpAsync`.
    -   In `StripeWebhookController.cs`, create `POST /api/webhooks/stripe`. This endpoint is also `[AllowAnonymous]` and will call `OnboardingService.HandleSuccessfulPaymentWebhookAsync`. It must be secured by validating the Stripe request signature.
6.  **UI (React/Client):**
    -   Create a multi-step sign-up form on the public-facing site.
    -   On form submission, call the `/signup` endpoint.
    -   On receiving a successful response (with the Stripe URL), redirect the user to the Stripe Checkout page.

### User Story: The Provisioning Engine
**As the** System, **I need to** automatically allocate the correct infrastructure based on the customer's selected tier.

**Technical Tasks:**
1.  **Application (Service):** In `ProvisioningService.cs`, implement `ProvisionTenantAsync(Guid tenantId)`.
    -   This method fetches the `Tenant` and its `SubscriptionTier`.
    -   It uses a `switch` statement on the tier to decide the provisioning path.
2.  **Infrastructure (Tier 1 - Grassroots):**
    -   The service simply updates the `Tenant` status to `Active` and sends a "Welcome" email. No infrastructure jobs are needed.
3.  **Infrastructure (Tier 2 - Professional):**
    -   The service enqueues a `ProvisionTier2DatabaseJob`.
    -   **`ProvisionTier2DatabaseJob`**: This background job (`IBackgroundJobService`) will execute a shell script (`.ps1`/`.sh`) that uses a cloud CLI (e.g., AWS CLI, Azure CLI) to create a new PostgreSQL database.
    -   On success, the script outputs the connection string. The job captures this and updates the `ConnectionString` field on the `Tenant` record.
    -   It then enqueues a second job: `RunMigrationsJob(tenantId, newConnectionString)`.
4.  **Infrastructure (Data Migration for Upgrade):**
    -   Create a `MigrateTenantDataJob(Guid tenantId, string sourceConn, string targetConn)`.
    -   This job will:
        -   Set `tenant.IsReadOnly = true`.
        -   Use `pg_dump` and `psql` command-line tools to copy data from the shared DB (filtering by `tenant_id`) to the new dedicated DB.
        -   Verify data integrity (e.g., row counts).
        -   Set `tenant.IsReadOnly = false`.

## 4. Code Specifications (Key Logic)

### `StripeWebhookController.cs` - Receiving Payment Confirmation

```csharp
[ApiController]
[Route("api/webhooks")]
public class StripeWebhookController : ControllerBase
{
    private readonly IOnboardingService _onboardingService;
    private readonly IConfiguration _config;

    public StripeWebhookController(IOnboardingService onboardingService, IConfiguration config)
    {
        _onboardingService = onboardingService;
        _config = config;
    }

    [HttpPost("stripe")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ConstructEvent(json,
            Request.Headers["Stripe-Signature"],
            _config["Stripe:WebhookSecret"]); // Secret from appsettings

        // Handle the checkout.session.completed event
        if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        {
            await _onboardingService.HandleSuccessfulPaymentWebhookAsync(stripeEvent);
        }

        return Ok();
    }
}
```

### `ProvisioningService.cs` - Tier-Based Job Enqueueing

```csharp
// Inside ProvisioningService.cs
public class ProvisioningService : IProvisioningService
{
    private readonly BaseDbContext _context;
    private readonly IBackgroundJobService _jobService;

    // ... constructor ...

    public async Task ProvisionTenantAsync(Guid tenantId)
    {
        var tenant = await _context.Tenants.FindAsync(tenantId);
        if (tenant == null) return;

        switch (tenant.SubscriptionTier)
        {
            case SubscriptionTier.Grassroots:
                // No infra job needed, just activate
                tenant.SubscriptionStatus = SubscriptionStatus.Active;
                await _context.SaveChangesAsync();
                // Enqueue email job
                _jobService.Enqueue<IEmailService>(s => s.SendWelcomeEmail(tenant.OwnerEmail));
                break;

            case SubscriptionTier.Professional:
                // Enqueue a job that runs a shell script to create the DB
                _jobService.Enqueue<ProvisioningJobs>(j => j.ProvisionTier2Database(tenantId));
                break;

            case SubscriptionTier.Enterprise:
                 // Enqueue a job that calls an external DevOps pipeline
                _jobService.Enqueue<ProvisioningJobs>(j => j.TriggerTier3Pipeline(tenantId));
                break;
        }
    }
}
```

### `ProvisioningJobs.cs` - Background Job Logic (Pseudo-code)

```csharp
public class ProvisioningJobs
{
    // Injected: DbContext, IConfiguration, etc.

    public async Task ProvisionTier2Database(Guid tenantId)
    {
        // 1. Execute shell script to create DB
        // var scriptPath = _config["Scripts:ProvisionDbPath"];
        // var newDbConnectionString = ExecuteShellCommand($"{scriptPath} --tenant-name {tenant.Name}");
        
        // 2. If successful, update tenant with new connection string
        // var tenant = await _context.Tenants.FindAsync(tenantId);
        // tenant.ConnectionString = newDbConnectionString;
        // await _context.SaveChangesAsync();

        // 3. Enqueue the next job to run migrations
        // _jobService.Enqueue<ProvisioningJobs>(j => j.RunMigrationsOnTenantDb(tenantId));
    }

    public async Task RunMigrationsOnTenantDb(Guid tenantId)
    {
        // 1. Get tenant connection string
        // 2. Execute shell command: `dotnet ef database update --connection "{connectionString}"`
        // 3. On success, update tenant status to Active and send email
    }
}
```