# Technical Implementation Guide: Background Jobs & Real-Time Notifications

**Version:** 1.0
**Date:** 2025-12-03
**Reference:** `docs/Business Requirements/BackgroundJobs_Hangfire_FRS.md`

## 1. Architectural Analysis

This implementation follows **Clean Architecture** principles to decouple the background processing technology (Hangfire) from the core business logic.

### 1.1. Layered Responsibility

| Layer | Component | Responsibility |
| :--- | :--- | :--- |
| **Domain** | `ProvisioningStatus` | Enum defining the state of a tenant (Pending, Provisioning, Active, Failed). |
| **Application** | `IBackgroundJobService` | **Abstraction.** Interface defining how to enqueue jobs. Does NOT reference Hangfire. |
| **Application** | `ProvisionTenantJob` | **Logic.** The actual business logic to execute (or a wrapper calling a Domain Service/Mediator). |
| **Infrastructure** | `HangfireService` | **Implementation.** Implements `IBackgroundJobService` using the Hangfire SDK. |
| **Infrastructure** | `NotificationHub` | **Real-Time.** SignalR Hub for managing connections and pushing updates. |
| **Web API** | `Startup/Program.cs` | Configuration of Hangfire Server, Dashboard, and SignalR endpoints. |

### 1.2. Abstraction (Application Layer)
We define the contract for background jobs in the Application layer. This allows us to swap Hangfire for Azure Functions or standard `Task.Run` in the future without changing business logic.

```csharp
// Diquis.Application/Interfaces/IBackgroundJobService.cs
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Diquis.Application.Interfaces
{
    public interface IBackgroundJobService
    {
        /// <summary>
        /// Enqueues a fire-and-forget job.
        /// </summary>
        /// <param name="methodCall">The method call to execute.</param>
        /// <returns>The unique Job ID.</returns>
        string Enqueue(Expression<Action> methodCall);

        /// <summary>
        /// Enqueues a fire-and-forget job that returns a Task.
        /// </summary>
        /// <param name="methodCall">The async method call to execute.</param>
        /// <returns>The unique Job ID.</returns>
        string Enqueue(Expression<Func<Task>> methodCall);
    }
}
```

### 1.3. Implementation (Infrastructure Layer)
The Infrastructure layer references the Hangfire libraries and implements the interface.

```csharp
// Diquis.Infrastructure/Services/HangfireJobService.cs
using Diquis.Application.Interfaces;
using Hangfire;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Diquis.Infrastructure.Services
{
    public class HangfireJobService : IBackgroundJobService
    {
        public string Enqueue(Expression<Action> methodCall)
        {
            return BackgroundJob.Enqueue(methodCall);
        }

        public string Enqueue(Expression<Func<Task>> methodCall)
        {
            return BackgroundJob.Enqueue(methodCall);
        }
    }
}
```

## 2. Domain Entities

### 2.1. Tenant Status
Update the `Tenant` entity to track the provisioning lifecycle.

```csharp
// Diquis.Domain/Enums/ProvisioningStatus.cs
public enum ProvisioningStatus
{
    Pending = 0,
    Provisioning = 1,
    Active = 2,
    Failed = 3
}
```

## 3. Job Logic (Application Layer)

The job itself orchestrates the provisioning process. It should be idempotent and handle failures gracefully.

```csharp
// Diquis.Application/Jobs/ProvisionTenantJob.cs
public class ProvisionTenantJob
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IDatabaseMigrator _migrator;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ProvisionTenantJob(
        ITenantRepository tenantRepository,
        IDatabaseMigrator migrator,
        IHubContext<NotificationHub> hubContext)
    {
        _tenantRepository = tenantRepository;
        _migrator = migrator;
        _hubContext = hubContext;
    }

    public async Task ExecuteAsync(Guid tenantId, string adminUserId)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        if (tenant == null) return;

        try
        {
            // 1. Update Status
            tenant.Status = ProvisioningStatus.Provisioning;
            await _tenantRepository.UpdateAsync(tenant);

            // 2. Heavy Lifting (Database Creation & Seeding)
            await _migrator.CreateAndMigrateTenantDbAsync(tenant.ConnectionString);

            // 3. Success
            tenant.Status = ProvisioningStatus.Active;
            await _tenantRepository.UpdateAsync(tenant);

            // 4. Notify Admin (Scenario A)
            if (!string.IsNullOrEmpty(adminUserId))
            {
                await _hubContext.Clients.User(adminUserId)
                    .SendAsync("ReceiveNotification", "Success", $"Tenant {tenant.Name} is ready.");
            }
            
            // 5. Send Email (Scenario B) - Logic omitted for brevity
        }
        catch (Exception ex)
        {
            tenant.Status = ProvisioningStatus.Failed;
            await _tenantRepository.UpdateAsync(tenant);
            
            // Notify Failure
            if (!string.IsNullOrEmpty(adminUserId))
            {
                await _hubContext.Clients.User(adminUserId)
                    .SendAsync("ReceiveNotification", "Error", $"Provisioning failed: {ex.Message}");
            }
            
            throw; // Re-throw to let Hangfire handle retries or move to DLQ
        }
    }
}
```

## 4. Security

### 4.1. Hangfire Dashboard
The dashboard allows full control over jobs, so it must be restricted to Super Admins.

**Implementation:**
Create an `IDashboardAuthorizationFilter`.

```csharp
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        // 1. Check if user is authenticated
        if (httpContext.User.Identity?.IsAuthenticated != true) return false;

        // 2. Check for Super Admin role
        return httpContext.User.IsInRole("SuperAdmin");
    }
}
```

**Configuration (Program.cs):**
```csharp
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new [] { new HangfireAuthorizationFilter() }
});
```

### 4.2. SignalR Authentication
WebSockets don't support custom headers easily, so the JWT token is often passed in the Query String.

**Configuration (Program.cs):**
```csharp
services.AddAuthentication(options => { ... })
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                // If the request is for our Hub...
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
```

## 5. Frontend Integration (React)

### 5.1. SignalR Hook (`useSignalR`)
A custom hook to manage the connection lifecycle and event listeners.

```typescript
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export const useSignalR = (hubUrl: string, token: string) => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                accessTokenFactory: () => token // Pass token for auth
            })
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, [hubUrl, token]);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => console.log('SignalR Connected'))
                .catch(err => console.error('SignalR Connection Error: ', err));

            // Listen for notifications
            connection.on('ReceiveNotification', (type, message) => {
                // Trigger Toast UI logic here
                console.log(`[${type}] ${message}`);
            });
        }
        
        return () => {
            connection?.stop();
        };
    }, [connection]);

    return connection;
};
```
