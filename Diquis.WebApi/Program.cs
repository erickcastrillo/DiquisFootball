using Diquis.Application.Common;
using Diquis.Infrastructure.BackgroundJobs;
using Diquis.WebApi.Extensions;
using Diquis.WebApi.Middleware;

using Hangfire;
using Hangfire.Console;

using Microsoft.OpenApi;

using OpenTelemetry.Trace;

using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApplicationServices(builder.Configuration); // Register Services / CORS / Configure Identity Requirements / JWT Settings / Register DB Contexts / Image Handling, Mailer, Fluent Validation, Automapper

// Add Hangfire Services
builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseConsole()); // Add Hangfire.Console

builder.Services.AddHangfireServer();
builder.Services.AddScoped<IBackgroundJobService, HangfireJobService>();
builder.Services.AddScoped<IJobClientWrapper, JobClientWrapper>();

// Setup OpenTelemetry Tracing
builder.Services.AddOpenTelemetry().WithTracing(builder =>
{
    _ = builder
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

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireAuthorizationFilter("Admin")]
});
app.UseMiddleware<TenantResolver>();
app.MapControllers();
app.MapFallbackToController("Index", "Fallback"); // directs all traffic to index.html

app.Run();
