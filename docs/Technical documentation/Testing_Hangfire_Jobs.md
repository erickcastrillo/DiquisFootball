# Testing Hangfire Job Scheduling from WebApi

## Overview
This document explains how to test Hangfire job scheduling from the Diquis.WebApi application.

## Test Endpoints

Three test endpoints have been added to verify Hangfire integration:

### 1. Get Job Status
**GET** `/api/test/job-status`

Returns information about Hangfire configuration and available test endpoints.

**Example Request:**
```bash
curl -X GET https://localhost:7183/api/test/job-status
```

**Example Response:**
```json
{
  "message": "Hangfire is configured and ready",
  "dashboardUrl": "https://localhost:7298/hangfire",
  "endpoints": [
    {
      "method": "POST",
      "path": "/api/test/enqueue-job",
      "description": "Enqueue a successful test job"
    },
    {
      "method": "POST",
      "path": "/api/test/enqueue-failing-job",
      "description": "Enqueue a failing test job"
    }
  ]
}
```

### 2. Enqueue Test Job (Success)
**POST** `/api/test/enqueue-job?message={your-message}`

Enqueues a test job that will succeed. The job will:
- Log messages to the console
- Simulate 5 seconds of work (1 second per iteration)
- Log progress at 20%, 40%, 60%, 80%, and 100%
- Complete successfully

**Example Request:**
```bash
curl -X POST "https://localhost:7183/api/test/enqueue-job?message=Testing Hangfire Integration"
```

**Example Response:**
```json
{
  "success": true,
  "jobId": "12345",
  "message": "Job enqueued successfully with ID: 12345",
  "checkDashboard": "Visit https://localhost:7298/hangfire to monitor the job"
}
```

### 3. Enqueue Test Job (Failure)
**POST** `/api/test/enqueue-failing-job`

Enqueues a test job that will fail intentionally. This is useful for testing:
- Error handling in Hangfire
- OpenTelemetry error tracking
- Failed job visualization in the dashboard

**Example Request:**
```bash
curl -X POST https://localhost:7183/api/test/enqueue-failing-job
```

**Example Response:**
```json
{
  "success": true,
  "jobId": "12346",
  "message": "Failing job enqueued with ID: 12346",
  "note": "This job will fail intentionally to test error handling and OpenTelemetry error tracking",
  "checkDashboard": "Visit https://localhost:7298/hangfire to see the failure"
}
```

## Testing via Swagger UI

1. Start the **Diquis.WebApi** application
2. Navigate to `https://localhost:7183/swagger`
3. Find the "Hangfire Testing" section
4. Try the endpoints:
   - Click "GET /api/test/job-status" ? "Try it out" ? "Execute"
   - Click "POST /api/test/enqueue-job" ? "Try it out" ? Enter a message ? "Execute"
   - Click "POST /api/test/enqueue-failing-job" ? "Try it out" ? "Execute"

## Monitoring Jobs

### Via Hangfire Dashboard

1. Start the **Diquis.BackgroundJobs** application
2. Navigate to `https://localhost:7298/`
3. Login with:
   - **Email:** `admin@job.com`
   - **Password:** `Password123!`
4. Click "Open Hangfire Dashboard" or navigate directly to `https://localhost:7298/hangfire`
5. You'll see:
   - **Enqueued Jobs:** Jobs waiting to be processed
   - **Processing Jobs:** Jobs currently being executed
   - **Succeeded Jobs:** Completed jobs
   - **Failed Jobs:** Jobs that threw exceptions

### Via Application Logs

When running Diquis.BackgroundJobs, you'll see console output like:

**Successful Job:**
```
info: Diquis.WebApi.Jobs.TestJob[0]
      Test job started with message: Testing Hangfire Integration
info: Diquis.WebApi.Jobs.TestJob[0]
      Test job progress: 20%
info: Diquis.WebApi.Jobs.TestJob[0]
      Test job progress: 40%
...
info: Diquis.WebApi.Jobs.TestJob[0]
      Test job completed successfully
```

**Failed Job:**
```
warn: Diquis.WebApi.Jobs.TestJob[0]
      Test job that will fail
fail: Hangfire.Server.BackgroundServerProcess[0]
      System.InvalidOperationException: This is a test exception to verify error tracking in OpenTelemetry
```

## OpenTelemetry Tracing

If you have an OTLP-compatible backend configured (e.g., Jaeger, Zipkin), you'll see:

1. **Job Creation Span**
   - Operation: `Hangfire Job Creating: TestJob.ExecuteAsync`
   - Tags: `hangfire.job.type`, `hangfire.job.method`, `hangfire.state`

2. **Job Execution Span**
   - Operation: `Hangfire Job Execution: TestJob.ExecuteAsync`
   - Tags: `hangfire.job.id`, `hangfire.server.name`, `hangfire.retry.count`
   - Events: State changes (Enqueued ? Processing ? Succeeded/Failed)
   - For failed jobs: Exception details are recorded

3. **HTTP Request Span**
   - The original HTTP POST request that triggered the job
   - Linked to the job creation span via trace context

## Architecture

```
???????????????????????????????????????????????????????????????
?                     Diquis.WebApi                           ?
?  (Port 7183)                                                ?
?                                                             ?
?  1. POST /api/test/enqueue-job                             ?
?     ?                                                       ?
?  2. IBackgroundJobService.Enqueue(...)                     ?
?     ?                                                       ?
?  3. Hangfire stores job in PostgreSQL                      ?
?     ?                                                       ?
?  4. Returns job ID to caller                               ?
???????????????????????????????????????????????????????????????
                         ?
                         ? (Shared PostgreSQL Database)
                         ?
                         ?
???????????????????????????????????????????????????????????????
?               Diquis.BackgroundJobs                    ?
?  (Port 7298)                                                ?
?                                                             ?
?  1. Hangfire Server polls for new jobs                     ?
?     ?                                                       ?
?  2. Picks up job from queue                                ?
?     ?                                                       ?
?  3. Executes TestJob.ExecuteAsync(...)                     ?
?     ?                                                       ?
?  4. Logs progress and completion                           ?
?     ?                                                       ?
?  5. Updates job status in PostgreSQL                       ?
?     ?                                                       ?
?  6. Sends telemetry to OTLP exporter                       ?
???????????????????????????????????????????????????????????????
```

## Troubleshooting

### Jobs Not Appearing in Dashboard
- Ensure both applications are running
- Verify they're using the same `HangfireConnection` connection string
- Check that PostgreSQL is running and accessible

### Jobs Stay in "Enqueued" State
- Verify `Diquis.BackgroundJobs` is running
- Check the console for errors in the Jobs application
- Ensure Hangfire Server is started (configured in `ServiceCollectionExtensions.cs`)

### Cannot Access Dashboard
- Navigate to `https://localhost:7298/` first
- Login with the root credentials
- If you get a 401, restart the Jobs application

### Jobs Execute but No Telemetry
- Verify OTLP exporter is configured in `appsettings.json`
- Check that your observability backend is running
- Look for `HangfireActivityFilter` in the telemetry data

## Next Steps

After verifying the test jobs work:

1. **Remove Test Endpoints** (in production):
   - Comment out or remove the test endpoints from `Program.cs`
   - Keep `TestJob.cs` for reference or delete it

2. **Create Real Jobs**:
   - Create job classes in `Diquis.WebApi/Jobs/` or `Diquis.Infrastructure/BackgroundJobs/`
   - Register them in the DI container
   - Enqueue them from your controllers or services

3. **Configure Recurring Jobs**:
   - Use `RecurringJob.AddOrUpdate` in `Program.cs` startup
   - Set up CRON expressions for scheduled tasks

4. **Monitor Production**:
   - Set up alerts for failed jobs
   - Review job execution times
   - Optimize job performance based on telemetry data
