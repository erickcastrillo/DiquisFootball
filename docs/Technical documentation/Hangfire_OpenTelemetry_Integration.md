# Hangfire OpenTelemetry Instrumentation

## Overview
This implementation provides distributed tracing for Hangfire background jobs using OpenTelemetry, allowing you to monitor job execution, track performance, and correlate job execution with HTTP requests.

## How It Works

### Architecture
The solution uses a shared custom Hangfire filter (`HangfireActivityFilter`) located in `Diquis.Infrastructure/BackgroundJobs/Telemetry/` that creates OpenTelemetry spans at different stages of job lifecycle:

1. **Job Creation (Producer)**: When a job is enqueued, a span is created to track the enqueue operation
2. **Job Execution (Consumer)**: When a job starts executing, a new span is created that links to the creation span
3. **State Changes**: Job state transitions are recorded as events in the active span
4. **Error Tracking**: Exceptions are captured and recorded with full stack traces

### Key Features

#### 1. Distributed Tracing
- Creates parent-child relationship between job creation and execution
- Allows tracing requests that span across HTTP calls and background jobs
- Preserves trace context across async operations

#### 2. Rich Metadata
Each span includes tags for:
- `hangfire.job.id`: Unique job identifier
- `hangfire.job.type`: Full type name of the job class
- `hangfire.job.method`: Method being executed
- `hangfire.server.name`: Server processing the job
- `hangfire.retry.count`: Number of retry attempts
- `hangfire.state.new/old`: State transitions

#### 3. Error Reporting
- Captures exceptions with full details
- Sets span status to Error when jobs fail
- Records exception details for debugging

## Configuration

### Shared Component Location
- **Filter**: `Diquis.Infrastructure/BackgroundJobs/Telemetry/HangfireActivityFilter.cs`
- This filter is shared across all projects that use Hangfire

### Projects Configured
1. **Diquis.BackgroundJobs**: Background job processing server
2. **Diquis.WebApi**: API that enqueues jobs

### Setup Steps

Both projects have been configured with:

1. **Project Reference**: `Diquis.BackgroundJobs` references `Diquis.Infrastructure`
2. **ActivitySource Registration**: Added `"Diquis.Hangfire"` source to OpenTelemetry
3. **Filter Registration**: Hangfire configured with `HangfireActivityFilter` from shared infrastructure
4. **Required Packages**:
   - `OpenTelemetry.Api` - For Activity extension methods (in Diquis.Infrastructure)
   - `OpenTelemetry.Exporter.OpenTelemetryProtocol` - OTLP exporter
   - `OpenTelemetry.Instrumentation.AspNetCore` - ASP.NET Core tracing

## Usage Example

When you enqueue a job:

```csharp
// In your API controller
_backgroundJobService.Enqueue(() => SomeService.ProcessData(tenantId));
```

OpenTelemetry will create a trace that shows:
1. The HTTP request that triggered the job
2. The job enqueue operation
3. The actual job execution (potentially on a different server)
4. Any errors or state changes

## Viewing Traces

Traces are exported via OTLP protocol. You can view them in:
- **Jaeger**: Distributed tracing UI
- **Zipkin**: Another popular tracing backend
- **Azure Application Insights**: If configured
- **Any OTLP-compatible backend**

### Sample Trace Structure
```
HTTP POST /api/tenants
?? Hangfire Job Creating: TenantService.ProvisionTenant
   ?? Hangfire Job Execution: TenantService.ProvisionTenant
      ?? Event: State Applied: Processing
      ?? Event: State Applied: Succeeded (or Failed)
      ?? Database operations, HTTP calls, etc.
```

## Benefits

1. **Performance Monitoring**: Track job execution time and identify slow operations
2. **Error Diagnosis**: See exactly where and why jobs fail
3. **Dependency Tracking**: Understand how jobs interact with databases, external APIs, etc.
4. **Production Debugging**: Trace specific requests through your entire system
5. **SLA Monitoring**: Track job completion times and success rates

## Best Practices

1. **Sensitive Data**: Avoid logging sensitive information in tags or events
2. **Cardinality**: Be careful with high-cardinality tags (e.g., unique IDs in tag names)
3. **Sampling**: Configure sampling for high-volume scenarios to reduce overhead
4. **Correlation**: Pass correlation IDs through job parameters for better tracking

## Future Enhancements

Consider adding:
- Custom activity tags for business-specific metadata
- Metrics for job queue depth and processing rate
- Sampling strategies for high-volume jobs
- Integration with other observability tools (Prometheus, Grafana)
