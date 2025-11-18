using Diquis.Application.Common;
using Diquis.Infrastructure.BackgroundJobs;
using Diquis.WebApi.Extensions;
using Diquis.WebApi.Middleware;
using Hangfire;
using OpenTelemetry.Trace;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApplicationServices(builder.Configuration); // Register Services / CORS / Configure Identity Requirements / JWT Settings / Register DB Contexts / Image Handling, Mailer, Fluent Validation, Automapper

// Add Hangfire Services
//builder.Services.AddHangfire(config => config
//    .UseSimpleAssemblyNameTypeSerializer()
//    .UseRecommendedSerializerSettings()
//    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))); // Or .UsePostgreSqlStorage(...)

//builder.Services.AddHangfireServer();
//builder.Services.AddScoped<IBackgroundJobService, HangfireJobService>();

// Setup OpenTelemetry Tracing
builder.Services.AddOpenTelemetry().WithTracing(builder =>
{
    _ = builder
        // Configure ASP.NET Core Instrumentation
        .AddAspNetCoreInstrumentation()
        // Configure OpenTelemetry Protocol (OTLP) Exporter
        .AddOtlpExporter();
});

WebApplication app = builder.Build(); // Create the App


app.UseCors("defaultPolicy"); // CORS policy (default - allow any orgin)
app.UseHttpsRedirection();

app.UseRouting();
app.UseDefaultFiles();  // enables serving static files from wwwroot folder (react client - index.html)
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
//app.UseHangfireDashboard("/hangfire", new DashboardOptions
//{
//    Authorization = [new HangfireAuthorizationFilter("Admin")]
//});
app.UseMiddleware<TenantResolver>();
app.MapControllers();
app.MapFallbackToController("Index", "Fallback"); // directs all traffic to index.html

app.Run();
