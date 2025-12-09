# Debugging SignalR Real-Time Notifications

## Step-by-Step Debugging Guide

### Step 1: Check Redis is Running

```bash
docker ps | findstr redis
```

Expected output: Should show redis container running on port 6379

### Step 2: Monitor Redis Activity

Open a new terminal and run:
```bash
docker exec -it <redis-container-name> redis-cli MONITOR
```

This will show all Redis commands in real-time. When a notification is sent, you should see messages with the prefix `Diquis`.

### Step 3: Restart Both Applications

**Important:** Restart in this order:

1. Stop Diquis.WebApi
2. Stop Diquis.BackgroundJobs  
3. Start Diquis.BackgroundJobs (wait for it to fully start)
4. Start Diquis.WebApi

### Step 4: Open Browser with Console

1. Open browser to http://localhost:3000/tenants
2. Open DevTools (F12)
3. Go to Console tab
4. Clear the console (Ctrl+L or click clear button)

### Step 5: Verify SignalR Connection

You should see in the browser console:

```
?? SignalR: Connecting to https://localhost:7250/hubs/notifications
?? SignalR: Token present: YES
?? SignalR: Registering event handlers...
? SignalR: All event handlers registered
?? SignalR: Registered events: ['Connected', 'TenantCreated', 'TenantCreationFailed', 'TenantUpdated', 'TenantUpdateFailed']
?? SignalR: Starting connection...
? ========== SIGNALR CONNECTED SUCCESSFULLY ==========
?? Connection ID: <some-guid>
?? Connection State: Connected
?? Hub URL: https://localhost:7250/hubs/notifications
======================================================
```

If you don't see this, check:
- Is backend running?
- Is the token valid? (try logging in again)
- Are there any red errors in console?

### Step 6: Test with Test Endpoint

Before creating a tenant, test the notification system:

1. In a new browser tab, navigate to:
   ```
   https://localhost:7250/api/test/signalr-notification
   ```

2. Go back to the tenants page tab and check the console

3. You should see:
   ```
   ?? ========== TENANT CREATED EVENT RECEIVED ==========
   ?? Full data received: {
     "type": "success",
     "message": "Tenant 'Test Tenant' has been created successfully!",
     "userId": "test-user",
     "tenantId": "test-tenant-123",
     "tenantName": "Test Tenant",
     "timestamp": "2024-01-15T10:30:00.000Z"
   }
   ?? Data type: object
   ?? Data keys: ["type", "message", "userId", "tenantId", "tenantName", "timestamp"]
   ====================================================
   ?? Refreshing tenant list...
   ```

**If you DON'T see this:**
- The issue is with SignalR connection or Redis backplane
- Check backend logs for errors
- Check Redis MONITOR output

**If you DO see this:**
- SignalR is working!
- The issue might be with the background job itself

### Step 7: Create Real Tenant

1. Click "Create Tenant"
2. Fill in the form:
   - ID: `debug-test`
   - Name: `Debug Test Academy`
   - Email: `admin@debug.com`
   - Password: `Test123!@#`
   - Has Isolated Database: Yes

3. Submit the form

### Step 8: Monitor All Logs

**Browser Console:**
Watch for the SignalR event. You should see one of:
- `?? ========== TENANT CREATED EVENT RECEIVED ==========`
- `? ========== TENANT CREATION FAILED EVENT RECEIVED ==========`

**Backend Logs (Diquis.BackgroundJobs):**
Look for:
```
========== SENDING SIGNALR NOTIFICATION ==========
Notification Type: TenantCreated
User ID: <user-id>
Tenant ID: debug-test
Tenant Name: Debug Test Academy
==================================================
Sending TenantCreated notification for tenant debug-test
SignalR notification sent successfully
```

**Backend Logs (Diquis.WebApi):**
May show SignalR Hub connection logs

**Redis Monitor:**
Should show messages being published and subscribed with prefix `Diquis`

## Common Issues and Solutions

### Issue 1: SignalR connects but no events received

**Check:**
```bash
# In browser console, after connection established:
connection.invoke("Ping")  // If hub has this method
```

**Solution:**
- Verify event names match exactly: `TenantCreated`, `TenantCreationFailed`, etc.
- Check Redis is actually being used (see MONITOR output)
- Ensure both apps have same `ChannelPrefix: "Diquis"`

### Issue 2: No logs in background jobs

**Check:**
- Is the job actually running? Check Hangfire dashboard: https://localhost:7298/hangfire
- Look for the provisioning job
- Check job status and logs

**Solution:**
- Job might have failed before reaching notification code
- Check for exceptions in Hangfire dashboard

### Issue 3: Redis not showing activity

**Check:**
```bash
docker exec -it <redis-container> redis-cli
127.0.0.1:6379> KEYS *
127.0.0.1:6379> PUBSUB CHANNELS
```

**Solution:**
- If no `Diquis*` channels exist, Redis backplane is not configured correctly
- Verify `ConnectionStrings:RedisConnection` in both appsettings.json
- Check for Redis connection errors in application logs

### Issue 4: SignalR won't connect

**Error in console:**
```
? ========== SIGNALR CONNECTION FAILED ==========
```

**Solutions:**
1. Check if backend is running
2. Verify hub URL is correct
3. Check CORS settings
4. Try logging in again (token might be expired)
5. Check backend logs for authentication errors

### Issue 5: Events received but UI not updating

**Check:**
- Is `store.tenantsStore.loadTenants()` being called?
- Check network tab for the API call
- Verify the tenants endpoint is working

**Solution:**
- The SignalR notification is working, but the refresh logic might have an issue
- Check for errors in `loadTenants()` method
- Manually refresh page to confirm tenant was actually created

## Verification Checklist

Run through this checklist:

- [ ] Redis is running: `docker ps | findstr redis`
- [ ] Redis MONITOR shows activity when test endpoint is called
- [ ] Browser console shows: `? ========== SIGNALR CONNECTED SUCCESSFULLY ==========`
- [ ] Test endpoint triggers notification in browser console
- [ ] Backend logs show: `========== SENDING SIGNALR NOTIFICATION ==========`
- [ ] Creating tenant shows same logs
- [ ] Browser console shows: `?? ========== TENANT CREATED EVENT RECEIVED ==========`
- [ ] Toast notification appears
- [ ] Tenant list refreshes

## Expected Full Flow

1. **User submits tenant creation form**
   - Browser sends POST to `/api/tenants`
   
2. **API enqueues background job**
   - Returns 202 Accepted immediately
   - Job queued in Hangfire
   
3. **Hangfire processes job**
   - `ProvisionTenantJob.ExecuteAsync()` runs
   - Creates admin user
   - Provisions database (if needed)
   - Updates tenant status to Active
   
4. **Job sends SignalR notification**
   - Backend logs: `========== SENDING SIGNALR NOTIFICATION ==========`
   - Calls `NotifyTenantCreatedAsync()`
   
5. **SignalR sends to Redis**
   - SignalRNotificationService logs: `Sending TenantCreated notification`
   - Message published to Redis channel `Diquis:...`
   
6. **Redis broadcasts to all apps**
   - Redis MONITOR shows: `PUBLISH "Diquis:..." ...`
   
7. **Main app receives from Redis**
   - WebApi's SignalR hub receives message from Redis
   
8. **Hub broadcasts to connected clients**
   - Message sent to all browser clients
   
9. **Browser receives event**
   - Console logs: `?? ========== TENANT CREATED EVENT RECEIVED ==========`
   - Shows full data received
   
10. **UI updates**
    - Toast notification appears
    - `loadTenants()` called
    - Tenant list refreshes

## If Nothing Works

1. Take screenshot of browser console (after SignalR connection)
2. Take screenshot of backend logs (Background Jobs)
3. Take screenshot of Redis MONITOR output
4. Share all three for further debugging

## Quick Test Script

Run this in browser console after SignalR connects:

```javascript
// Check connection state
console.log('Connection state:', connection.state);

// List all registered handlers
console.log('Registered handlers:', connection._callbacks);

// Manually trigger test
fetch('https://localhost:7250/api/test/signalr-notification')
  .then(r => r.json())
  .then(d => console.log('Test endpoint response:', d));
```

You should see the notification event fire immediately.
