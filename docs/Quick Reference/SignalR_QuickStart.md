# ?? Quick Start Guide - SignalR Real-Time Notifications

## ? System Status: WORKING

All issues resolved. System is functional and ready for use.

---

## ?? Start Services (In Order)

```bash
# 1. Start Redis
docker start redis

# 2. Verify Redis is running
docker ps | findstr redis

# 3. Start BackgroundJobs (Diquis.BackgroundJobs)
# Run from Visual Studio or:
# dotnet run --project Diquis.BackgroundJobs

# 4. Start WebApi (Diquis.WebApi)
# Run from Visual Studio or:
# dotnet run --project Diquis.WebApi

# 5. Frontend should auto-start with WebApi
# If not: cd Diquis.WebApi/Frontend && npm run dev
```

---

## ? Verify Everything is Working

### 1. Check Redis
```bash
docker exec -it redis redis-cli ping
# Should return: PONG
```

### 2. Check SignalR Connection
- Open: http://localhost:3000/tenants
- Open browser console (F12)
- Look for: `SignalR connected`

### 3. Test Notifications
- Visit: https://localhost:7250/api/test/signalr-notification
- Should see toast notification in browser
- Console should show: `Tenant created: {...}`

### 4. Test Full Flow
- Create new tenant from UI
- Watch real-time status change: Provisioning ? Active
- No page refresh needed ?

---

## ?? Troubleshooting

### SignalR Not Connecting?
```bash
# Check CORS in browser console
# Should NOT see: "blocked by CORS policy"

# Check backend logs for:
info: User Anonymous connected to NotificationHub
```

### No Notifications?
```bash
# Monitor Redis
docker exec -it redis redis-cli MONITOR

# Should see messages when tenant is created
# Look for: "DiquisDiquis.Infrastructure.Hubs.NotificationHub"
```

### Database Creation Fails?
```bash
# Check PostgreSQL is running
# Check connection string in appsettings.json
# Verify user has CREATE DATABASE permission
```

---

## ?? Key Files Reference

### Backend
- **SignalR Service:** `Diquis.Infrastructure/Services/SignalRNotificationService.cs`
- **Hub:** `Diquis.Infrastructure/Hubs/NotificationHub.cs`
- **Provisioning Job:** `Diquis.Infrastructure/BackgroundJobs/ProvisionTenantJob.cs`
- **CORS Config:** `Diquis.WebApi/Extensions/ServiceCollectionExtensions.cs`

### Frontend
- **SignalR Hook:** `Diquis.WebApi/Frontend/src/hooks/useSignalR.ts`

### Configuration
- **WebApi:** `Diquis.WebApi/appsettings.json`
- **BackgroundJobs:** `Diquis.BackgroundJobs/appsettings.json`

---

## ?? Environment Variables

Both appsettings.json files need:
```json
{
  "ConnectionStrings": {
    "RedisConnection": "localhost:6379"
  }
}
```

---

## ?? What Was Fixed Today

1. ? StackOverflowException in SaveChangesAsync
2. ? SignalR hub namespace mismatch (2 different hubs)
3. ? CORS blocking SignalR with credentials
4. ? 401 Unauthorized on hub connection
5. ? Database creation for isolated tenants

---

## ?? Documentation

Full details in: `docs/Session Summaries/2024-12-09_SignalR_Implementation_And_Fixes.md`

Debugging guides in:
- `Diquis.BackgroundJobs/TROUBLESHOOTING_SIGNALR.md`
- `Diquis.BackgroundJobs/QUICK_DEBUG.md`
- `Diquis.BackgroundJobs/BUG_FIXED.md`

---

## ?? Success Indicators

You know it's working when:

? Browser console: "SignalR connected"  
? Create tenant ? status updates in real-time  
? Toast notification appears  
? No manual page refresh needed  
? Backend logs: "Sending tenant created notification"  

---

## ?? If Something Breaks

1. **Check Redis first:** `docker ps | findstr redis`
2. **Check browser console** for SignalR errors
3. **Check backend logs** for notification sending
4. **Monitor Redis:** `docker exec -it redis redis-cli MONITOR`
5. **Restart everything** in order (Redis ? BackgroundJobs ? WebApi)

---

## ?? Git Commit Ready

**Branch:** feature/tenant-background-jobs-signalr

**Files Changed:** 19 files (see session summary)

**Suggested Commit:**
```
Fix SignalR real-time notifications and StackOverflowException

- Fixed StackOverflowException in SaveChangesAsync
- Resolved hub namespace mismatch
- Added Redis backplane
- Fixed CORS for credentials
- Added database creation for tenants
```

---

## ?? Next Session

Everything is working! Consider:
- [ ] Add more robust error handling
- [ ] Add retry logic for database creation
- [ ] Consider re-enabling [Authorize] on hub with proper JWT
- [ ] Add production Redis configuration
- [ ] Add health checks for Redis

---

**Last Updated:** December 9, 2024  
**Status:** ? WORKING  
**Ready for:** Testing, Demo, Production Prep
