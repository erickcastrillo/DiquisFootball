# SignalR Real-Time Notifications - Troubleshooting Guide

## Issue: Real-time notifications not working, need to refresh page

## Root Cause Analysis

The issue was that the **main application (Diquis.WebApi)** was missing the Redis backplane configuration. 
While the background jobs project had Redis configured, both applications need to use the same Redis instance 
for cross-process SignalR messaging to work.

## Fix Applied

### 1. Added Redis Backplane to Main Application

**File**: `Diquis.WebApi/Program.cs`

```csharp
// Get Redis connection string
string redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";

// Add SignalR with Redis backplane for cross-process messaging
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConnectionString, options =>
    {
        options.Configuration.ChannelPrefix = "Diquis";
    });
```

### 2. Enhanced SignalR Notification Service with Logging

**File**: `Diquis.Infrastructure/Services/SignalRNotificationService.cs`

- Added logging to track when notifications are sent
- Added proper `message` field that frontend expects
- Improved error handling

### 3. Added Test Endpoint

```
GET /api/test/signalr-notification
```

Use this to verify SignalR is working without creating a tenant.

## Testing Steps

### Step 1: Verify Redis is Running

```bash
docker ps | findstr redis
```

You should see Redis container running on port 6379.

### Step 2: Check Redis Connection

```bash
docker exec -it <redis-container-id> redis-cli ping
```

Should return: `PONG`

### Step 3: Restart Both Applications

1. Stop both Diquis.WebApi and Diquis.BackgroundJobs
2. Start Redis (if not already running):
   ```bash
   docker run -d -p 6379:6379 --name redis redis:alpine
   ```
3. Start Diquis.BackgroundJobs
4. Start Diquis.WebApi
5. Open browser to http://localhost:3000

### Step 4: Test SignalR Connection

1. Open browser DevTools (F12)
2. Go to Console tab
3. Look for: `? SignalR Connected`

If you see connection errors, check:
- Is the backend running?
- Is the token valid?
- Is the hub URL correct?

### Step 5: Test Real-Time Notifications

**Option A: Use Test Endpoint**

Open browser and navigate to:
```
https://localhost:7250/api/test/signalr-notification
```

Check browser console for:
```
?? Tenant Created: {type: 'success', message: '...', ...}
```

**Option B: Create Actual Tenant**

1. Go to Tenants page: http://localhost:3000/tenants
2. Click "Create Tenant"
3. Fill in form and submit
4. Watch browser console for real-time updates
5. Status should update from "Provisioning" to "Active" without refresh

## Expected Flow

```
???????????????????????
?  1. User creates    ?
?     tenant via UI   ?
???????????????????????
           ?
           ?
???????????????????????
?  2. API enqueues    ?
?     Hangfire job    ?
???????????????????????
           ?
           ?
???????????????????????
?  3. Background Job  ?
?     processes       ?
?     provision       ?
???????????????????????
           ?
           ?
???????????????????????
?  4. Job calls       ?
?     notification    ?
?     service         ?
???????????????????????
           ?
           ?
???????????????????????
?  5. SignalR sends   ?
?     to Redis        ?
???????????????????????
           ?
           ?
???????????????????????
?  6. Redis forwards  ?
?     to main app     ?
???????????????????????
           ?
           ?
???????????????????????
?  7. Main app's      ?
?     SignalR hub     ?
?     broadcasts      ?
???????????????????????
           ?
           ?
???????????????????????
?  8. Browser receives?
?     & updates UI    ?
???????????????????????
```

## Debugging Tips

### Check Backend Logs

**Main Application (Diquis.WebApi)**
Look for:
```
Sending TenantCreated notification for tenant test-tenant-123
```

**Background Jobs**
Look for:
```
Tenant tenant-id provisioned successfully
```

### Check Browser Console

Expected messages:
```
?? SignalR: Connecting to https://localhost:7250/hubs/notifications
? SignalR Connected
?? Tenant Created: {...}
```

### Check Redis Messages

Monitor Redis in real-time:
```bash
docker exec -it <redis-container-id> redis-cli
127.0.0.1:6379> MONITOR
```

You should see messages with prefix `Diquis` when notifications are sent.

### Verify Both Apps Use Same Redis

Both `appsettings.json` files should have:
```json
{
  "ConnectionStrings": {
    "RedisConnection": "localhost:6379"
  }
}
```

### Check SignalR Hub is Mapped

In `Diquis.WebApi/Program.cs`:
```csharp
app.MapHub<NotificationHub>("/hubs/notifications");
```

In `Diquis.BackgroundJobs/Program.cs`:
```csharp
app.MapHub<NotificationHub>("/hubs/notifications");
```

## Common Issues

### Issue: SignalR connects but no notifications received

**Cause**: Message format mismatch between backend and frontend

**Fix**: Verify notification includes `message` field:
```csharp
await _hubContext.Clients.All.SendAsync("TenantCreated", new
{
    type = "success",
    message = "Tenant created successfully!", // Required by frontend
    userId,
    tenantId,
    tenantName,
    timestamp = DateTime.UtcNow.ToString("o")
});
```

### Issue: Redis connection refused

**Cause**: Redis not running

**Fix**: 
```bash
docker start redis
# or
docker run -d -p 6379:6379 --name redis redis:alpine
```

### Issue: CORS errors in browser

**Cause**: Frontend and backend on different origins

**Fix**: Already configured in `ConfigureApplicationServices` with `defaultPolicy`

### Issue: 401 Unauthorized on SignalR connection

**Cause**: JWT token expired or missing

**Fix**: Login again to get fresh token

## Verification Checklist

- [ ] Redis is running and accessible on port 6379
- [ ] Both applications have Redis connection string configured
- [ ] Both applications have SignalR with Redis backplane configured
- [ ] Both applications have hub mapped at `/hubs/notifications`
- [ ] Backend logs show "Sending TenantCreated notification"
- [ ] Browser console shows "SignalR Connected"
- [ ] Test endpoint `/api/test/signalr-notification` triggers browser notification
- [ ] Creating actual tenant shows real-time status update

## Performance Tuning

If you experience delays:

1. **Reduce Hangfire polling interval** (already set to 1 second):
   ```csharp
   options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
   ```

2. **Increase Redis connection pool**:
   ```csharp
   .AddStackExchangeRedis(redisConnectionString, options =>
   {
       options.Configuration.ChannelPrefix = "Diquis";
       options.Configuration.ConnectionPoolSize = 10; // Default is usually sufficient
   });
   ```

3. **Monitor Redis performance**:
   ```bash
   docker exec -it redis redis-cli INFO stats
   ```

## Production Considerations

### Use Managed Redis Service

Replace `localhost:6379` with:

**Azure Redis Cache:**
```json
"RedisConnection": "your-cache.redis.cache.windows.net:6380,password=xxx,ssl=True,abortConnect=False"
```

**AWS ElastiCache:**
```json
"RedisConnection": "your-cluster.xxx.cache.amazonaws.com:6379"
```

### Enable SSL

```csharp
.AddStackExchangeRedis(redisConnectionString, options =>
{
    options.Configuration.ChannelPrefix = "Diquis";
    options.Configuration.Ssl = true; // For production
    options.Configuration.AbortOnConnectFail = false; // Prevent startup failures
});
```

### Monitor Redis Health

Add health checks:
```csharp
builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString);
```

## Success Indicators

When everything is working correctly, you should see:

1. **Browser Console:**
   ```
   ?? SignalR: Connecting to https://localhost:7250/hubs/notifications
   ? SignalR Connected
   [When tenant is created]
   ?? Tenant Created: {type: 'success', message: 'Tenant created...', tenantId: '...'}
   ```

2. **Backend Logs:**
   ```
   [Information] Tenant tenant-id provisioned successfully
   [Information] Sending TenantCreated notification for tenant tenant-id
   ```

3. **UI Behavior:**
   - Tenant status updates from "Provisioning" ? "Active" without page refresh
   - Toast notification appears with success message
   - Tenant list automatically refreshes

## Need Help?

If issues persist:

1. Check all logs (backend, browser console, Redis)
2. Verify all configuration files
3. Test with the `/api/test/signalr-notification` endpoint first
4. Ensure both applications are running
5. Check firewall/network settings (especially in Docker/cloud environments)
