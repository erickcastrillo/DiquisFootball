# OpenTelemetry & Honeycomb.io Setup Guide for .NET

This guide explains how to use OpenTelemetry instrumentation with Honeycomb.io as the observability backend for the Diquis ASP.NET Core API.

## Overview

OpenTelemetry (OTel) is an open-source observability framework that provides:

-   **Distributed Tracing**: Track requests across services.
-   **Metrics**: Collect performance data.
-   **Logs**: Structured logging with context.

Honeycomb.io is a powerful observability platform that receives and analyzes this telemetry data.

## What's Instrumented

The following components are automatically instrumented by the OpenTelemetry .NET SDK:

### ASP.NET Core Framework
- ✅ Incoming HTTP requests and responses.
- ✅ Controller actions or Minimal API endpoints.
- ✅ Unhandled exceptions.

### External Services
- ✅ Outgoing HTTP client requests (`HttpClient`).
- ✅ Entity Framework Core database queries (with SQL text).
- ✅ Redis operations (via `StackExchange.Redis`).
- ✅ Hangfire or Quartz.NET background jobs.

### Custom Application Code
- ✅ Request metadata (user, tenant, IP, etc.) via custom instrumentation.
- ✅ Custom service layer and business logic instrumentation.

## Setup Instructions

### 1. Get Your Honeycomb.io API Key

1.  Sign up at [Honeycomb.io](https://www.honeycomb.io/).
2.  Create a new environment or use an existing one.
3.  Go to **Environment Settings** > **API Keys**.
4.  Create a new API key or copy an existing one.

### 2. Configure the Application

**a. Install NuGet Packages:**
```bash
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.EntityFrameworkCore
```

**b. Configure in `Program.cs`:**
```csharp
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Define a resource builder for your service
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: "Diquis.Api", serviceVersion: "1.0.0");

// Add OpenTelemetry services
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
        tracerProviderBuilder
            .SetResourceBuilder(resourceBuilder)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = true; // Capture SQL text
            })
            .AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri("https://api.honeycomb.io");
                otlpOptions.Headers = $"x-honeycomb-team={builder.Configuration["Honeycomb:ApiKey"]},x-honeycomb-dataset=diquis-dotnet";
            }));

// ... rest of your Program.cs
```

**c. Configure `appsettings.json` or User Secrets:**
```json
{
  "Honeycomb": {
    "ApiKey": "YOUR_API_KEY_HERE",
    "Dataset": "diquis-development"
  },
  "OTEL_SERVICE_NAME": "Diquis.Api"
}
```

### 3. Run the Application
```bash
dotnet run --project Diquis.WebApi
```
You should see trace information being output to the console if you have the console exporter configured, and telemetry data will start flowing to Honeycomb.

### 4. View Traces in Honeycomb
1.  Go to your Honeycomb.io environment.
2.  Select your dataset (e.g., `diquis-development`).
3.  You should see traces appearing in real-time as you interact with your API.

## Custom Instrumentation

To add custom tracing to your application logic, you can inject an `ActivitySource`.

### Example in a Service
```csharp
using System.Diagnostics;

public class PlayerService
{
    private static readonly ActivitySource ActivitySource = new ActivitySource("Diquis.Application.PlayerService");
    private readonly IRepository<Player> _repository;

    public PlayerService(IRepository<Player> repository)
    {
        _repository = repository;
    }

    public async Task<Player> CreatePlayerAsync(CreatePlayerRequest request)
    {
        // Start a new activity (span)
        using var activity = ActivitySource.StartActivity("CreatePlayer");

        try
        {
            // Add custom attributes (tags)
            activity?.SetTag("player.name", $"{request.FirstName} {request.LastName}");
            activity?.SetTag("academy.id", request.AcademyId);

            var player = new Player(...);
            await _repository.AddAsync(player);

            // Record an event
            activity?.AddEvent(new ActivityEvent("PlayerCreated", tags: new ActivityTagsCollection
            {
                { "player.id", player.Id },
                { "player.age", player.Age }
            }));

            return player;
        }
        catch (Exception ex)
        {
            // Record exceptions
            activity?.RecordException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, "Failed to create player");
            throw;
        }
    }
}
```
You also need to add the `ActivitySource` to your OpenTelemetry configuration in `Program.cs`:
```csharp
// In .WithTracing(...)
.AddSource("Diquis.Application.PlayerService")
```

## Honeycomb.io Best Practices

### 1. Use Queries to Find Issues
**Slow Database Queries:**
```
WHERE db.system = "postgresql"
GROUP BY db.statement
ORDER BY P99(duration_ms) DESC
```

**Failed Requests:**
```
WHERE http.status_code >= 500
GROUP BY http.route
ORDER BY COUNT DESC
```

### 2. Set Up Service Level Objectives (SLOs)
Define performance targets in Honeycomb to monitor the health of your application, such as:
-   95% of requests complete in < 200ms.
-   99.9% of requests return a successful status code.

### 3. Create Triggers for Alerts
Set up alerts for critical issues like:
-   Spikes in the error rate.
-   High request latency.
-   Database query performance degradation.

## Troubleshooting

### No Traces Appearing in Honeycomb
1.  **Check API Key and Dataset**: Ensure the `ApiKey` and `Dataset` in your configuration are correct.
2.  **Check Console Output**: Look for any errors from the OpenTelemetry exporter in your application's console logs.
3.  **Verify Network**: Make sure your application can reach `https://api.honeycomb.io`.

### High Data Volume / Costs
-   **Sampling**: In production, you may want to sample traces to reduce data volume. This can be configured in the `TracerProviderBuilder`.
    ```csharp
    .SetSampler(new TraceIdRatioBasedSampler(0.1)) // Sample 10% of traces
    ```
-   **Filtering**: You can filter out certain requests (like health checks) in the `AddAspNetCoreInstrumentation` options.

## Resources
- [OpenTelemetry .NET Documentation](https://opentelemetry.io/docs/instrumentation/net/)
- [Honeycomb.io .NET Documentation](https://docs.honeycomb.io/getting-data-in/net/)
