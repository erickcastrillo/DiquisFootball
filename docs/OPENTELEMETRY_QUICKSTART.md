# OpenTelemetry Quick Start for .NET

This guide provides a quick overview of how to add custom tracing to your .NET application using OpenTelemetry.

## Enable Tracing

1.  **Install Packages**: Ensure you have the necessary OpenTelemetry packages installed.
    ```bash
    dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
    dotnet add package OpenTelemetry.Extensions.Hosting
    dotnet add package OpenTelemetry.Instrumentation.AspNetCore
    ```

2.  **Configure `Program.cs`**: Set up the OpenTelemetry tracer provider and export it to Honeycomb.
    ```csharp
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracerProviderBuilder =>
            tracerProviderBuilder
                .AddSource("Diquis.Api") // Your custom ActivitySource name
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri("https://api.honeycomb.io");
                    otlpOptions.Headers = "x-honeycomb-team=YOUR_API_KEY,x-honeycomb-dataset=diquis-dotnet";
                }));
    ```

3.  **Configure `appsettings.json`**:
    ```json
    {
      "Honeycomb": {
        "ApiKey": "YOUR_API_KEY_HERE"
      }
    }
    ```

4.  **Run the app**:
    ```bash
    dotnet run --project Diquis.WebApi
    ```

5.  **View Traces**: Visit [https://ui.honeycomb.io/](https://ui.honeycomb.io/) to see your data.

## Add Custom Tracing to Your Code

In .NET, custom tracing is done using `System.Diagnostics.Activity` and `ActivitySource`.

### 1. Create an `ActivitySource`

Create a static `ActivitySource` instance in your service or a central helper class. The name should be unique to the component you are instrumenting.

```csharp
using System.Diagnostics;

public static class TelemetrySources
{
    public static readonly ActivitySource PlayerServiceSource = new("Diquis.Application.PlayerService");
}
```
Remember to add this source name to your OpenTelemetry configuration in `Program.cs`: `.AddSource("Diquis.Application.PlayerService")`.

### 2. Track Custom Operations (Spans)

Wrap your business logic in a `using` statement with a new activity.

```csharp
public class PlayerService
{
    public async Task<Player> CreatePlayerAsync(CreatePlayerRequest request)
    {
        // Start a new activity (span)
        using var activity = TelemetrySources.PlayerServiceSource.StartActivity("CreatePlayer");

        // Add custom attributes (tags)
        activity?.SetTag("player.name", $"{request.FirstName} {request.LastName}");
        activity?.SetTag("academy.id", request.AcademyId);

        var player = new Player(...);
        // ... save player ...

        // Track the result
        activity?.SetTag("player.id", player.Id);
        activity?.SetTag("player.created", true);

        return player;
    }
}
```

### 3. Track Business Events

Add events to an activity to mark significant points in time during an operation.

```csharp
public async Task ProcessPaymentAsync(decimal amount, string currency)
{
    using var activity = TelemetrySources.PaymentServiceSource.StartActivity("ProcessPayment");

    activity?.SetTag("payment.amount", amount);
    activity?.SetTag("payment.currency", currency);

    var result = await _stripeClient.ChargeAsync(amount, currency);

    // Add an event milestone
    var eventTags = new ActivityTagsCollection
    {
        { "payment.transaction_id", result.TransactionId },
        { "payment.success", result.IsSuccess }
    };
    activity?.AddEvent(new ActivityEvent("PaymentProcessed", tags: eventTags));
}
```

### 4. Track Errors and Exceptions

Record exceptions within an activity to easily find and debug errors.

```csharp
public async Task ImportDataAsync(Stream fileStream)
{
    using var activity = TelemetrySources.ImportServiceSource.StartActivity("ImportData");
    try
    {
        // ... processing logic ...
    }
    catch (Exception ex)
    {
        // Record the exception
        activity?.RecordException(ex);
        // Set the status of the activity to Error
        activity?.SetStatus(ActivityStatusCode.Error, "Data import failed");
        throw; // Re-throw the exception
    }
}
```

### 5. Track in Controllers/Endpoints

You can add custom attributes in your API endpoints to capture request-specific context.

```csharp
[HttpPost]
public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequest request)
{
    // The ASP.NET Core instrumentation automatically creates an activity for the request.
    // We can add tags to the current activity.
    Activity.Current?.SetTag("tenant.id", HttpContext.Items["TenantId"]);
    Activity.Current?.SetTag("user.id", User.FindFirstValue(ClaimTypes.NameIdentifier));

    var player = await _playerService.CreatePlayerAsync(request);
    return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
}
```

## Best Practices

✅ **DO:**
-   Use consistent, namespaced naming for attributes (e.g., `player.id`, `academy.name`).
-   Track business-relevant data that provides context.
-   Record exceptions to correlate errors with traces.
-   Use events to mark important milestones within a longer operation.

❌ **DON'T:**
-   Add Personally Identifiable Information (PII) like passwords or credit card numbers to traces.
-   Use high-cardinality data (like a unique request ID) as an attribute *name*. Attribute *values* can be high-cardinality.
-   Create spans for every single method call; focus on meaningful units of work.

## Querying in Honeycomb

Once you've added custom attributes, you can use them in Honeycomb queries:

**Find slow player creation by academy:**
```
WHERE name = "CreatePlayer"
GROUP BY academy.id
ORDER BY P99(duration_ms) DESC
```

**Calculate payment success rate:**
```
WHERE name = "ProcessPayment"
VISUALIZE COUNT, AVG(payment.success)
GROUP BY payment.currency
```

## Learn More

See the full [OpenTelemetry & Honeycomb Setup Guide](./OPENTELEMETRY_HONEYCOMB_SETUP.md) for complete documentation.
