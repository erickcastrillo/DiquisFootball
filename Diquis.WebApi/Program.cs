using Diquis.Application.Common;
using Diquis.Application.Common.BackgroundJobs;
using Diquis.Infrastructure.BackgroundJobs;
using Diquis.Infrastructure.BackgroundJobs.Telemetry;
using Diquis.WebApi.Extensions;
using Diquis.WebApi.Middleware;

using Hangfire;
using Hangfire.Console;
using Hangfire.PostgreSql;

using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.OpenApi;

using OpenTelemetry.Trace;

using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApplicationServices(builder.Configuration); // Register Services / CORS / Configure Identity Requirements / JWT Settings / Register DB Contexts / Image Handling, Mailer, Fluent Validation, Automapper

// Add Hangfire Services
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180) // Good practice to be explicit
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HangfireConnection"));
    })
    .UseFilter(new HangfireActivityFilter()));

builder.Services.AddScoped<IBackgroundJobService, HangfireJobService>();
builder.Services.AddScoped<IJobClientWrapper, JobClientWrapper>();

// Register test job
builder.Services.AddScoped<TestJob>();

// Setup OpenTelemetry Tracing
builder.Services.AddOpenTelemetry().WithTracing(tracingBuilder =>
{
    _ = tracingBuilder
        // Add Hangfire instrumentation via ActivitySource
        .AddSource("Diquis.Hangfire")
        // Configure ASP.NET Core Instrumentation
        .AddAspNetCoreInstrumentation()
        // Configure OpenTelemetry Protocol (OTLP) Exporter
        .AddOtlpExporter();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Diquis API Documentation",
        Version = "v1",
        Description = "A comprehensive ASP.NET Core 10.0+ API for managing football academies, players, teams, and training sessions with a multi-tenant architecture."
    });

    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });

    // using System.Reflection;
    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

WebApplication app = builder.Build(); // Create the App

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("defaultPolicy"); // CORS policy (default - allow any orgin)
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();  // enables serving static files from wwwroot folder (react client - index.html)
app.UseStaticFiles();

app.UseMiddleware<TenantResolver>();
app.MapControllers();

// Test endpoints for Hangfire job scheduling
app.MapGet("/api/test/enqueue-job", (IBackgroundJobService jobService, string message = "Hello from Hangfire!") =>
{
    var jobId = jobService.Enqueue(() => new TestJob(default!).ExecuteAsync(message, null));
    return Results.Ok(new
    {
        success = true,
        jobId,
        message = $"Job enqueued successfully with ID: {jobId}",
        checkDashboard = "Visit https://localhost:7298/hangfire to monitor the job"
    });
})
.WithName("EnqueueTestJob")
.WithTags("Hangfire Testing")
.WithSummary("Enqueue a test Hangfire job")
.WithDescription("Enqueues a simple test job to verify Hangfire is working. The job will log messages and simulate 5 seconds of work.");

app.MapGet("/api/test/enqueue-failing-job", (IBackgroundJobService jobService) =>
{
    var jobId = jobService.Enqueue(() => new TestJob(default!).ExecuteWithErrorAsync(null));
    return Results.Ok(new
    {
        success = true,
        jobId,
        message = $"Failing job enqueued with ID: {jobId}",
        note = "This job will fail intentionally to test error handling and OpenTelemetry error tracking",
        checkDashboard = "Visit https://localhost:7298/hangfire to see the failure"
    });
})
.WithName("EnqueueFailingTestJob")
.WithTags("Hangfire Testing")
.WithSummary("Enqueue a test job that fails")
.WithDescription("Enqueues a test job that will fail to verify error handling and OpenTelemetry error tracking.");

app.MapGet("/api/test/job-status", () =>
{
    return Results.Ok(new
    {
        message = "Hangfire is configured and ready",
        dashboardUrl = "https://localhost:7298/hangfire",
        endpoints = new[]
        {
            new { method = "POST", path = "/api/test/enqueue-job", description = "Enqueue a successful test job" },
            new { method = "POST", path = "/api/test/enqueue-failing-job", description = "Enqueue a failing test job" }
        }
    });
})
.WithName("GetJobStatus")
.WithTags("Hangfire Testing")
.WithSummary("Get Hangfire status and test endpoints")
.WithDescription("Returns information about available test endpoints and Hangfire dashboard URL.");

app.MapFallbackToController("Index", "Fallback"); // directs all traffic to index.html

app.Run();
