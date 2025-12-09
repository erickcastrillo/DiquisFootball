# Redis Backplane Configuration Instructions

## 1. Add NuGet Package

Add the Redis package to **both** projects:

### Diquis.BackgroundJobs
```bash
dotnet add Diquis.BackgroundJobs package Microsoft.AspNetCore.SignalR.StackExchangeRedis
```

### Your Main Application Project
```bash
dotnet add YourMainApp package Microsoft.AspNetCore.SignalR.StackExchangeRedis
```

## 2. Update appsettings.json

Add Redis connection string to **both** `appsettings.json` files:

### Diquis.BackgroundJobs/appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "...",
    "HangfireConnection": "...",
    "RedisConnection": "localhost:6379"
  }
}
```

### Main Application appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "...",
    "RedisConnection": "localhost:6379"
  }
}
```

## 3. Configure Main Application

Add this to your **main application's** `Program.cs` or startup configuration:

```csharp
using Diquis.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Get Redis connection
string redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";

// Add SignalR with Redis backplane
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConnectionString, options =>
    {
        options.Configuration.ChannelPrefix = "Diquis";
    });

// ... your other services ...

var app = builder.Build();

// ... middleware configuration ...

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hub - IMPORTANT: Add this line!
app.MapHub<NotificationHub>("/hubs/notifications");

// ... rest of your configuration ...

app.Run();
```

## 4. Map SignalR Hub in Background Jobs

Add this to `Diquis.BackgroundJobs/Program.cs` (if not already present):

```csharp
using Diquis.Infrastructure.Services;

// ... after app.UseRouting() ...

app.MapHub<NotificationHub>("/hubs/notifications");
```

## 5. Start Redis (Docker)

```bash
docker run -d -p 6379:6379 --name redis redis:alpine
```

Or with persistence:
```bash
docker run -d -p 6379:6379 --name redis -v redis-data:/data redis:alpine redis-server --appendonly yes
```

## 6. Frontend JavaScript Integration

Add this to your Razor Page that needs real-time tenant updates:

```html
@section Scripts {
<script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
<script>
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/notifications")
        .withAutomaticReconnect()
        .build();

    connection.on("TenantCreated", (data) => {
        console.log("? Tenant created:", data);
        toastr.success(`Tenant "${data.tenantName}" created successfully!`);
        
        // Update UI - refresh tenant list or update specific row
        location.reload(); // Simple approach, or use AJAX to update specific elements
    });

    connection.on("TenantCreationFailed", (data) => {
        console.error("? Tenant creation failed:", data);
        toastr.error(`Failed to create tenant: ${data.error}`);
    });

    connection.on("TenantUpdated", (data) => {
        console.log("? Tenant updated:", data);
        toastr.success(`Tenant "${data.tenantName}" updated successfully!`);
        location.reload();
    });

    connection.on("TenantUpdateFailed", (data) => {
        console.error("? Tenant update failed:", data);
        toastr.error(`Failed to update tenant: ${data.error}`);
    });

    connection.start()
        .then(() => console.log("?? SignalR connected"))
        .catch(err => console.error("? SignalR connection error:", err));
</script>
}
```

## 7. Production Configuration

For production, use a managed Redis service and update connection string:

### Azure Redis Cache
```json
"RedisConnection": "your-redis.redis.cache.windows.net:6380,password=yourpassword,ssl=True,abortConnect=False"
```

### AWS ElastiCache
```json
"RedisConnection": "your-redis.cache.amazonaws.com:6379"
```

### Environment Variables
```bash
export ConnectionStrings__RedisConnection="your-redis-host:6379"
```

## Testing the Setup

1. Start Redis: `docker run -d -p 6379:6379 redis:alpine`
2. Start Background Jobs application
3. Start Main application
4. Open browser to main app
5. Create a new tenant
6. Watch browser console for SignalR messages
7. Tenant status should update in real-time!

## Troubleshooting

### SignalR not connecting
- Check that hub is mapped: `app.MapHub<NotificationHub>("/hubs/notifications");`
- Verify browser console for connection errors
- Check that SignalR JavaScript library is loaded

### No notifications received
- Verify Redis is running: `docker ps | grep redis`
- Check Redis connection: `redis-cli ping` (should return PONG)
- Look for errors in application logs
- Ensure both apps use same `ChannelPrefix: "Diquis"`

### CORS issues
If frontend and backend on different ports, add CORS:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
```

## Architecture

```
???????????????????         ???????????????         ????????????????????
?  Background Job ?????????>?    Redis    ?????????>?   Main App       ?
?  (Hangfire)     ?         ?  Backplane  ?         ?   SignalR Hub    ?
???????????????????         ???????????????         ????????????????????
                                                              ?
                                                              ?
                                                     ????????????????????
                                                     ?  Browser Clients ?
                                                     ?  (JavaScript)    ?
                                                     ????????????????????
```

When a background job completes:
1. Job calls `_notificationService.NotifyTenantCreatedAsync(...)`
2. SignalR sends message to Redis
3. Redis broadcasts to all connected apps (including main app)
4. Main app's SignalR hub pushes to browser clients
5. Browser receives real-time update!
