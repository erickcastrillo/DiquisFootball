# Task 01: Infrastructure - Install Hangfire & Create IJobService

**Status:** Open
**Priority:** High
**Context:** We are introducing background processing to offload long-running tasks. This task focuses on installing the necessary dependencies and defining the abstraction layer to decouple our application from the specific background job provider (Hangfire).

## 1. Dependencies
*   **Project:** `Diquis.Infrastructure`
    *   Install-Package `Hangfire.Core`
    *   Install-Package `Hangfire.PostgreSql`
*   **Project:** `Diquis.WebApi`
    *   Install-Package `Hangfire.AspNetCore`

## 2. Abstraction (Application Layer)
*   **File:** `Diquis.Application/Interfaces/IBackgroundJobService.cs`
*   **Action:** Define the interface to allow enqueuing jobs without referencing Hangfire types directly in the Application layer.
    ```csharp
    public interface IBackgroundJobService
    {
        string Enqueue(Expression<Action> methodCall);
        string Enqueue(Expression<Func<Task>> methodCall);
    }
    ```

## 3. Implementation (Infrastructure Layer)
*   **File:** `Diquis.Infrastructure/Services/HangfireJobService.cs`
*   **Action:** Implement `IBackgroundJobService` using `Hangfire.BackgroundJob`.
*   **Action:** Register this service in the DI container (likely in `InfrastructureServiceRegistration.cs` or `Program.cs`).

## 4. Validation
*   Build the solution to ensure no compilation errors.
*   Verify `IBackgroundJobService` can be injected into a controller (even if not used yet).
