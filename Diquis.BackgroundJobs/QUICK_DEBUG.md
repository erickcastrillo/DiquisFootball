# SignalR Debugging Quick Reference

## ?? What to Check Now

### 1. Open Browser Console (F12) on Tenants Page

Look for this exact text:
```
? ========== SIGNALR CONNECTED SUCCESSFULLY ==========
```

**? If you see it:** SignalR connection is working, go to step 2

**? If you don't see it:** 
- Try logging in again (token might be expired)
- Check if backend is running
- Look for red errors in console

### 2. Test the Notification System

Open new tab and go to:
```
https://localhost:7250/api/test/signalr-notification
```

Then go back to tenants page console.

**? If you see this:**
```
?? ========== TENANT CREATED EVENT RECEIVED ==========
?? Full data received: {...}
```
**? SignalR is working! Issue is with the background job.**

**? If you don't see anything:**
**? SignalR/Redis issue. Follow step 3.**

### 3. Check Redis is Actually Being Used

Open terminal and run:
```bash
docker exec -it <redis-container-name> redis-cli MONITOR
```

Then trigger the test endpoint again (step 2).

**? If you see messages with "Diquis" prefix:**
**? Redis is working but message not reaching browser. Check step 4.**

**? If you see NO activity:**
**? Redis backplane not configured correctly. Check step 5.**

### 4. Verify Both Apps Use Redis

**Check Diquis.WebApi/Program.cs:**
```csharp
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConnectionString, options =>
    {
        options.Configuration.ChannelPrefix = "Diquis";
    });
```

**Check Diquis.BackgroundJobs/Extensions/ServiceCollectionExtensions.cs:**
```csharp
services.AddSignalR()
    .AddStackExchangeRedis(redisConnectionString, options =>
    {
        options.Configuration.ChannelPrefix = "Diquis";
    });
```

Both must have:
- Same Redis connection string
- Same ChannelPrefix: "Diquis"

### 5. Check appsettings.json

**Both files must have:**

`Diquis.WebApi/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "RedisConnection": "localhost:6379"
  }
}
```

`Diquis.BackgroundJobs/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "RedisConnection": "localhost:6379"
  }
}
```

### 6. Restart Both Apps (IN THIS ORDER)

```
1. Stop Diquis.WebApi
2. Stop Diquis.BackgroundJobs
3. Start Diquis.BackgroundJobs (wait for full startup)
4. Start Diquis.WebApi
5. Refresh browser page
```

## ?? Decision Tree

```
Is Redis running?
?? NO ? Start Redis: docker run -d -p 6379:6379 redis:alpine
?? YES
   ?
   Is SignalR connected? (Check browser console)
   ?? NO ? Check token, backend running, errors in console
   ?? YES
      ?
      Does test endpoint work? (https://localhost:7250/api/test/signalr-notification)
      ?? YES ? Background job issue. Check Hangfire dashboard.
      ?? NO
         ?
         Does Redis MONITOR show activity?
         ?? YES ? Message routing issue. Check ChannelPrefix matches.
         ?? NO ? Redis backplane not configured. Check both Program.cs files.
```

## ?? Most Common Issues

### Issue: Test endpoint works, but tenant creation doesn't

**Cause:** Background job not calling notification service

**Fix:** Check Hangfire dashboard (https://localhost:7298/hangfire)
- Look for failed jobs
- Check job logs for errors
- Verify job reached the notification code

### Issue: Nothing in Redis MONITOR

**Cause:** Redis backplane not added to one or both apps

**Fix:** 
1. Check both `Program.cs` / `ServiceCollectionExtensions.cs` have `.AddStackExchangeRedis()`
2. Restart both apps
3. Try test endpoint again

### Issue: Browser not receiving events

**Cause:** Event name mismatch or handlers not registered

**Fix:**
- Check frontend registers handler: `connection.on('TenantCreated', ...)`
- Check backend sends: `SendAsync("TenantCreated", ...)`
- Names must match exactly (case-sensitive!)

## ? Success Indicators

When everything works, you'll see:

**Browser Console:**
```
? SIGNALR CONNECTED SUCCESSFULLY
[After test endpoint or tenant creation]
?? ========== TENANT CREATED EVENT RECEIVED ==========
?? Full data received: {"type":"success", "message":"...", ...}
?? Refreshing tenant list...
```

**Backend Logs (Background Jobs):**
```
========== SENDING SIGNALR NOTIFICATION ==========
Notification Type: TenantCreated
Sending TenantCreated notification for tenant <id>
SignalR notification sent successfully
```

**Redis MONITOR:**
```
[timestamp] "PUBLISH" "Diquis:..." "..."
```

**UI:**
- Toast notification appears
- Tenant status updates without refresh
- No need to manually reload page

## ?? Emergency Reset

If nothing works, do a complete reset:

```bash
# 1. Stop everything
# Stop both .NET apps

# 2. Reset Redis
docker stop redis
docker rm redis
docker run -d -p 6379:6379 --name redis redis:alpine

# 3. Clean and rebuild
dotnet clean
dotnet build

# 4. Start in order
# Start Diquis.BackgroundJobs
# Start Diquis.WebApi

# 5. Clear browser cache
# Ctrl+Shift+Delete ? Clear cache
# Or use Incognito/Private mode

# 6. Login again
# Go to http://localhost:3000
```

## ?? What to Share for Help

If still not working, share these 3 screenshots:

1. **Browser console** (after page load and test endpoint call)
2. **Background Jobs logs** (console output)
3. **Redis MONITOR output** (when test endpoint is called)

This will show exactly where the message flow breaks.
