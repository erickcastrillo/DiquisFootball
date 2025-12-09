# Quick Testing Guide: Tenant Background Jobs & SignalR

## ?? Quick Start

### 1. Start Both Services

**Terminal 1 - WebApi:**
```bash
cd Diquis.WebApi
dotnet run
```
? Opens on: `https://localhost:7250`

**Terminal 2 - Background Jobs Worker:**
```bash
cd Diquis.BackgroundJobs
dotnet run
```
? Opens on: `https://localhost:7298`

### 2. Access Dashboards

**Swagger UI (API Testing):**
```
https://localhost:7250/swagger
```

**Hangfire Dashboard (Job Monitoring):**
```
https://localhost:7298/
? Login with root user
? Click "Open Hangfire Dashboard"
```

---

## ?? Test Scenarios

### Scenario 1: Create Tenant (Success)

**1. Call API**
```bash
POST https://localhost:7250/api/tenants
Content-Type: application/json
Authorization: Bearer {your-jwt-token}

{
  "id": "demo-academy",
  "name": "Demo Academy",
  "adminEmail": "admin@demo.com",
  "password": "Test123!@#",
  "hasIsolatedDatabase": false
}
```

**2. Expected Response: 202 Accepted**
```json
{
  "succeeded": true,
  "data": "demo-academy",
  "messages": [
    "Tenant creation initiated. The tenant will be provisioned in the background."
  ]
}
```

**3. Monitor in Hangfire**
- Go to `https://localhost:7298/hangfire`
- Click "Jobs" ? "Processing"
- Watch job logs in real-time

**4. Verify Database**
```sql
SELECT id, name, status, provisioning_error
FROM tenants
WHERE id = 'demo-academy';
```
Expected: `status = 2` (Active)

**5. SignalR Event (when frontend is integrated)**
```json
{
  "type": "success",
  "message": "Tenant 'Demo Academy' has been successfully created and provisioned.",
  "tenantId": "demo-academy",
  "tenantName": "Demo Academy",
  "timestamp": "2024-12-09T..."
}
```

---

### Scenario 2: Create Tenant (Failure - Duplicate)

**1. Call API (with existing tenant ID)**
```bash
POST https://localhost:7250/api/tenants

{
  "id": "demo-academy",  // Already exists
  "name": "Duplicate Academy",
  "adminEmail": "admin2@demo.com",
  "password": "Test123!@#",
  "hasIsolatedDatabase": false
}
```

**2. Expected Response: 400 Bad Request**
```json
{
  "succeeded": false,
  "message": "Tenant already exists",
  "data": null
}
```

---

### Scenario 3: Update Tenant (Success)

**1. Call API**
```bash
PUT https://localhost:7250/api/tenants/demo-academy
Content-Type: application/json
Authorization: Bearer {your-jwt-token}

{
  "name": "Updated Demo Academy",
  "isActive": true
}
```

**2. Expected Response: 202 Accepted**
```json
{
  "succeeded": true,
  "data": "demo-academy",
  "messages": [
    "Tenant update initiated. The tenant will be updated in the background."
  ]
}
```

**3. Monitor in Hangfire**
- Watch `UpdateTenantJob.ExecuteAsync` job
- Verify completion

**4. Verify Database**
```sql
SELECT id, name, status
FROM tenants
WHERE id = 'demo-academy';
```
Expected: `name = 'Updated Demo Academy'`, `status = 2` (Active)

**5. SignalR Event**
```json
{
  "type": "success",
  "message": "Tenant 'Updated Demo Academy' has been successfully updated.",
  "tenantId": "demo-academy",
  "tenantName": "Updated Demo Academy",
  "timestamp": "2024-12-09T..."
}
```

---

### Scenario 4: Update Root Tenant (Failure)

**1. Call API**
```bash
PUT https://localhost:7250/api/tenants/root

{
  "name": "Hacked Root",
  "isActive": true
}
```

**2. Expected Response: 400 Bad Request**
```json
{
  "succeeded": false,
  "message": "Cannot edit root tenant",
  "data": null
}
```

---

## ?? Monitoring Checklist

### Hangfire Dashboard
- [ ] Navigate to `https://localhost:7298/hangfire`
- [ ] Check "Jobs" ? "Succeeded" count increases
- [ ] View job details (parameters, duration, logs)
- [ ] Check "Servers" tab shows 1 server running

### Application Logs
**WebApi Console:**
```
[INF] Tenant creation initiated for: demo-academy
[INF] Background job enqueued with ID: {jobId}
```

**BackgroundJobs Console:**
```
[INF] Starting provisioning for tenant demo-academy
[INF] Provisioning isolated database for tenant demo-academy
[INF] Tenant demo-academy provisioned successfully
[INF] Notifying user {userId} about successful tenant creation
```

### Database
```sql
-- Check tenant status
SELECT id, name, status, created_on, last_provisioning_attempt
FROM tenants
ORDER BY created_on DESC;

-- Status enum values:
-- 0 = Pending
-- 1 = Provisioning
-- 2 = Active
-- 3 = Failed
-- 4 = Updating
```

---

## ??? Troubleshooting

### Problem: Job stuck in "Enqueued" state
**Solution:**
- Check BackgroundJobs worker is running
- Check Hangfire connection string in appsettings.json
- Restart BackgroundJobs worker

### Problem: Job fails immediately
**Solution:**
- Check BackgroundJobs console for error logs
- View job details in Hangfire dashboard
- Check database connection strings
- Verify user credentials are valid

### Problem: No SignalR notifications
**Solution:**
- Verify frontend is connected to SignalR hub
- Check browser console for connection errors
- Verify JWT token is being passed correctly
- Check WebApi console for notification logs

### Problem: Cannot access Hangfire dashboard
**Solution:**
- Verify you're logged in as root user
- Check URL is `https://localhost:7298/hangfire` (not 7250)
- Clear browser cookies and re-login
- Check `HangfireAuthorizationFilter` is configured correctly

---

## ?? Test Data

### Valid Test Tenants
```json
{
  "id": "academy-one",
  "name": "Academy One",
  "adminEmail": "admin@academy-one.com",
  "password": "Test123!@#",
  "hasIsolatedDatabase": false
}
```

```json
{
  "id": "academy-two",
  "name": "Academy Two with Isolated DB",
  "adminEmail": "admin@academy-two.com",
  "password": "Test123!@#",
  "hasIsolatedDatabase": true
}
```

### Invalid Test Cases
```json
// Missing required fields
{
  "id": "bad-academy"
  // Missing name, adminEmail, password
}
```

```json
// Weak password
{
  "id": "weak-academy",
  "name": "Weak Academy",
  "adminEmail": "admin@weak.com",
  "password": "123"  // Too weak
}
```

---

## ? Success Criteria

- [x] Both services start without errors
- [x] API returns 202 Accepted for create/update
- [x] Job appears in Hangfire dashboard
- [x] Job completes successfully
- [x] Tenant status updates in database
- [x] Logs show detailed progress
- [x] SignalR notifications sent (when frontend connected)

---

## ?? Need Help?

**Check logs in:**
- WebApi console
- BackgroundJobs console
- Hangfire dashboard (job details)

**Common log locations:**
- Application logs: Console output
- Hangfire logs: Dashboard ? Jobs ? [Job ID]
- Database: Check `tenants` table directly

**Useful SQL queries:**
```sql
-- All tenants with status
SELECT id, name, status, provisioning_error
FROM tenants;

-- Failed tenants only
SELECT id, name, provisioning_error, last_provisioning_attempt
FROM tenants
WHERE status = 3;

-- Provisioning history
SELECT id, name, status, last_provisioning_attempt
FROM tenants
ORDER BY last_provisioning_attempt DESC;
```
