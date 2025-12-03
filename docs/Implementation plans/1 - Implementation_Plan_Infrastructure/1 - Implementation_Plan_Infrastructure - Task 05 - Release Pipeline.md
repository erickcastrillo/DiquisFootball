# Task Context
Implement the Continuous Deployment (CD) pipeline. This pipeline manages the deployment of the application to the shared environment (staging -> production swap) and orchestrates parallel deployments to isolated Enterprise environments.

# Core References
- **Plan:** [1 - Implementation_Plan_Infrastructure.md](./1%20-%20Implementation_Plan_Infrastructure.md)
- **Tech Guide:** [CommercialOnboarding_TechnicalGuide.md](../../Technical%20documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md)
- **FRS:** [CommercialOnboarding_FRS.md](../../Business%20Requirements/CommercialOnboarding_FRS/CommercialOnboarding_FRS.md)

# Step-by-Step Instructions
1.  **Create Workflow File:** Create `.github/workflows/release.yml`.
2.  **Define Trigger:** `workflow_dispatch` with input `image_tag`.
3.  **Job `deploy_shared`:**
    *   Deploy `image_tag` to the **Staging** slot of the Shared App Service.
    *   (Optional) Manual intervention step.
    *   Swap Staging to Production.
4.  **Job `deploy_enterprise`:**
    *   Needs `deploy_shared`.
    *   Use `matrix` strategy to deploy to multiple enterprise tenants in parallel.
    *   (For this task, you can mock the matrix or use a script to fetch active enterprise tenants).
    *   Deploy `image_tag` to the specific App Service for each tenant in the matrix.

# Acceptance Criteria
- [ ] File `.github/workflows/release.yml` exists.
- [ ] Pipeline handles Shared Environment deployment with slot swap.
- [ ] Pipeline includes structure for Matrix Deployment to Enterprise tenants.
