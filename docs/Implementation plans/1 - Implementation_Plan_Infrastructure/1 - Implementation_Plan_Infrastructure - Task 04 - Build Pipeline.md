# Task Context
Implement the Continuous Integration (CI) pipeline using GitHub Actions. This pipeline will trigger on code pushes, build the .NET solution, run tests, and containerize the application, pushing the Docker image to Azure Container Registry.

# Core References
- **Plan:** [1 - Implementation_Plan_Infrastructure.md](./1%20-%20Implementation_Plan_Infrastructure.md)
- **Tech Guide:** [CommercialOnboarding_TechnicalGuide.md](../../Technical%20documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md)
- **FRS:** [CommercialOnboarding_FRS.md](../../Business%20Requirements/CommercialOnboarding_FRS/CommercialOnboarding_FRS.md)

# Step-by-Step Instructions
1.  **Create Workflow File:** Create `.github/workflows/build.yml`.
2.  **Define Triggers:**
    *   `push` to `main`.
    *   `workflow_dispatch`.
3.  **Define Job `build_and_push`:**
    *   **Checkout:** `actions/checkout@v3`.
    *   **Setup .NET:** `actions/setup-dotnet@v3` (Version 10.0.x).
    *   **Restore & Build:** `dotnet restore` and `dotnet build -c Release`.
    *   **Test:** `dotnet test --no-build`.
    *   **Docker Login:** `azure/docker-login@v1` using secrets `ACR_NAME`, `ACR_USERNAME`, `ACR_PASSWORD`.
    *   **Build & Push:**
        *   `docker build . -f Diquis.WebApi/Dockerfile -t ${{ secrets.ACR_NAME }}.azurecr.io/api:${{ github.sha }}`
        *   `docker push ...`

# Acceptance Criteria
- [ ] File `.github/workflows/build.yml` exists.
- [ ] Pipeline includes Restore, Build, Test, and Docker Push steps.
- [ ] Docker image is tagged with commit SHA and `latest`.
