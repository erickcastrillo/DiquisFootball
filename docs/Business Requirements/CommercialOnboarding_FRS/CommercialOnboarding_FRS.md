## Module 10: 💳 Commercial Onboarding & Provisioning

### 1. Executive Summary
This document outlines the functional requirements for the commercial onboarding and automated provisioning engine of the Diquis platform. It details the hybrid-tenancy model, which supports both a cost-effective, multi-tenant architecture for self-service clients and a fully isolated infrastructure for enterprise partners, directly tying the business model to the technical implementation.

### 2. Key Actors
- **`Prospective Client`**: An individual, typically an academy owner or administrator, looking to purchase a subscription.
- **`Academy Owner`**: The administrative user for an active tenant, responsible for managing their subscription.
- **`System Super-Admin`**: A high-level internal user with rights to manage all tenants and manually provision infrastructure.
- **`Sales Agent`**: An internal team member responsible for managing enterprise contracts.

### 3. Functional Capabilities

#### Feature: Public Self-Service Onboarding (Tiers 1 & 2)
- **User Story:** "As a small/mid-sized Academy Owner, I want to select a plan, pay with a credit card, and get immediate access to the platform so I can start operations without talking to a sales representative."
- **Detailed Workflow:**
    - **Trigger:** A `Prospective Client` clicks a "Start Free Trial" or "Choose Plan" button on the public marketing website.
    - **Process:**
        1. **Account Creation:** The user enters their name, email address, and a password.
        2. **Email Verification:** The system sends a verification link to the provided email. The user must click this link to proceed.
        3. **Tier Selection:** The user is presented with the features and pricing for Tier 1 (Grassroots) and Tier 2 (Professional) and selects a plan.
        4. **Stripe Checkout:** The user is directed to a secure Stripe payment form to enter their credit card information and confirm the subscription purchase.
        5. **Provisioning Trigger:** Upon successful payment confirmation from Stripe, the system initiates the automated provisioning process for the selected tier. The user is shown a "Your academy is being set up" screen.
    - **Data Rules:**
        - Passwords must meet complexity requirements (e.g., minimum 8 characters, one uppercase, one number).
        - The email address must be unique across all user accounts in the system.
    - **Edge Cases:**
        - **Payment Failure (Hard Decline):** If Stripe returns a definitive failure, the user is shown a message: "Your payment could not be processed. Please check your card details or try a different card." No account is provisioned.
        - **Payment Failure (Soft Decline):** If the decline is temporary, the system may allow the user to retry once before showing a hard decline message.

#### Feature: Enterprise Onboarding Workflow (Tier 3)
- **User Story:** "As a representative from a national Federation or a World Class club, I need to work with a sales team to establish a custom contract and be provisioned with a fully isolated, high-performance instance of the platform."
- **Detailed Workflow:**
    - **Trigger:** An internal `Sales Agent` marks a deal as "Closed/Won" in the company's CRM (e.g., Salesforce), which notifies the `System Super-Admin`.
    - **Process:**
        1. The `System Super-Admin` logs into the super-admin dashboard and navigates to the "Tenants" section.
        2. The admin clicks "Create Enterprise Tenant" and fills out a form with the client's details (e.g., Organization Name, `Academy Owner` user info).
        3. The admin uploads a PDF copy of the signed `Contract`.
        4. The admin manually configures any specific requirements, such as a custom subdomain (e.g., `myclub.diquis.com`).
        5. The admin sets the `Contract End Date`.
        6. Upon saving, the system validates the form and triggers the Tier 3 provisioning process.
    - **Validation:**
        - The system will prevent the form from being submitted if the `Contract End Date` is less than 2 years from the current date, showing an error: "Enterprise contracts must have a minimum duration of 2 years."

#### Feature: The Provisioning Engine (Architecture Logic)
- **User Story:** "As the System, I need to automatically and correctly allocate the appropriate infrastructure based on the customer's selected subscription tier to ensure performance, isolation, and cost-effectiveness."
- **Detailed Business Logic:**
    - **If Tier 1 (Grassroots) is selected:**
        1. Create a new `tenant_id` and associated records in the central `tenants` table of the **Shared Database**.
        2. Create the `Academy Owner` user account.
        3. The application logic is inherently multi-tenant and will use the `tenant_id` to scope all data. No new infrastructure is created.
        4. Send a "Welcome" email to the user with a link to log in.
    - **If Tier 2 (Professional) is selected:**
        1. Trigger a cloud automation script.
        2. The script spins up a **New, Dedicated Database Instance** (e.g., a new AWS RDS for PostgreSQL instance).
        3. Upon successful creation, the script retrieves the new database's connection string.
        4. The connection string is securely injected into the tenant's configuration record in the central control-plane database.
        5. A background job runs the database migration scripts on the newly created database to establish the schema.
        6. Send a "Welcome" email.
    - **If Tier 3 (World Class) is selected:**
        1. Trigger a dedicated DevOps pipeline (e.g., via a webhook to Jenkins or GitLab CI).
        2. The pipeline executes a series of Terraform or Ansible scripts to deploy a **Fully Isolated Infrastructure Stack** (e.g., a dedicated VPC with its own application servers, database server, and networking).
        3. The pipeline configures DNS to point the client's custom subdomain to the new infrastructure.
        4. Upon completion, the pipeline notifies the `System Super-Admin` and `Sales Agent`.
    - **Edge Cases:**
        - **Provisioning Timeout:** If the process takes longer than expected (e.g., > 5 minutes), the user interface remains on the "Setting up..." screen. The backend process continues. Upon completion, the user is notified via email.
        - **Rollback Logic:** If any step of the provisioning fails (e.g., database creation fails in Tier 2), the system must automatically roll back. The user account is marked as "Provisioning Failed," any created resources are destroyed, the payment is voided/refunded, and an alert is sent to both the user and the system administrators.

#### Feature: Subscription Management & Upgrades
- **User Story:** "As an `Academy Owner` of a growing club, I want a seamless, self-service way to upgrade my plan from Tier 1 (Grassroots) to Tier 2 (Professional) to get better performance."
- **Detailed Workflow:**
    - **Process:**
        1. The `Academy Owner` navigates to their "Billing" or "Subscription" portal within the application.
        2. They select the "Upgrade to Professional" option.
        3. The system calculates and displays a prorated charge for the remainder of the billing cycle.
        4. Upon confirmation, the charge is processed via Stripe.
        5. On successful payment, the system triggers the **Data Migration Process**. The user is notified that the upgrade is in progress and may involve a brief maintenance period.
    - **Data Migration Trigger (Tier 1 to Tier 2):**
        1. The system puts the user's academy into a temporary, read-only "Maintenance Mode". Any user from that tenant attempting to perform a write action receives a friendly error message.
        2. A background job is initiated, following the same logic as a new Tier 2 provisioning to create a new dedicated database.
        3. The job then carefully copies all data associated with that `tenant_id` from the shared database to the new dedicated database.
        4. The tenant's configuration is updated with the new database's connection string.
        5. The "Maintenance Mode" flag is removed. The academy is now running on a dedicated database.
    - **Edge Case: Downtime Handling:**
        - During the data migration (which should be scheduled for off-peak hours where possible), any user from that tenant who logs in or is active in the app will see a banner: "We are upgrading your academy for better performance. The app is temporarily in read-only mode. Full functionality will be restored shortly."

### 4. The Feature Flag Matrix
This matrix defines which modules are accessible at each subscription tier.

| Module Name                          | Tier 1 Access (Grassroots) | Tier 2 Access (Professional) | Tier 3 Access (World Class) |
|--------------------------------------|----------------------------|------------------------------|-----------------------------|
| 1. Academy Operations                | Full                       | Full                         | Full                        |
| 2. Player Management                 | Full                       | Full                         | Full                        |
| 3. Team Organization                 | Full                       | Full                         | Full                        |
| 4. Training Sessions                 | Full                       | Full                         | Full                        |
| 5. Analytics & Reporting             | Basic                      | Advanced                     | Full + Custom Exports       |
| 6. Asset Management                  | Full                       | Full                         | Full                        |
| 7. **Sports Medicine & Bio-Passport**| `Disabled`                 | Full                         | Full                        |
| 8. **Scouting & Recruitment**        | Basic                      | Full                         | Full                        |
| 9. Facility & Resource Operations    | Full                       | Full                         | Full                        |

- **Basic:** Core features available (e.g., view dashboards, log a trialist).
- **Advanced:** Includes more complex features (e.g., advanced filtering, evaluation cards).
- **Full:** All features of the module are enabled.

### 5. Acceptance Criteria
- [ ] A new client can successfully subscribe to the Tier 1 plan via self-service and access their academy on the shared database.
- [ ] A new client can successfully subscribe to the Tier 2 plan, and a dedicated database is automatically provisioned for them.
- [ ] A `System Super-Admin` can use the admin dashboard to manually trigger the provisioning of a fully isolated Tier 3 instance.
- [ ] An `Academy Owner` on Tier 1 can upgrade to Tier 2, and their data is successfully and completely migrated to a new dedicated database.
- [ ] A user on a Tier 1 plan cannot see or access the "Sports Medicine" module.
- [ ] A user on a Tier 2 plan has full access to the "Sports Medicine" module.
- [ ] If a self-service payment fails, the user is notified, and no resources are provisioned.
- [ ] If a Tier 2 provisioning process fails, the database is destroyed, and the user's subscription is cancelled/refunded.
