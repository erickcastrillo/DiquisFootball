# ?? Quick Start: Testing Tenant Background Jobs + SignalR

**Status:** ? Ready to Test  
**Time Required:** 15-30 minutes

---

## 1?? Start Services (2 minutes)

### Terminal 1: WebApi
```bash
cd Diquis.WebApi
dotnet run
```
? Opens on: `https://localhost:7250`

### Terminal 2: Background Jobs Worker
```bash
cd Diquis.BackgroundJobs
dotnet run
```
? Opens on: `https://localhost:7298`

### Terminal 3: Frontend
```bash
cd Diquis.WebApi/Frontend
npm run serve
```
? Opens on: `http://localhost:3000` (or auto-opens in browser)

---

## 2?? Login (1 minute)

1. Navigate to: `http://localhost:3000`
2. Login as root user
3. Open browser DevTools ? Console

**Expected Console Output:**
```
?? SignalR: Connecting to https://localhost:7250/hubs/notifications
? SignalR Connected
?? SignalR: Successfully connected to notification hub
```

---

## 3?? Test Create Tenant (2 minutes)

### Steps:
1. Click "Tenants" in sidebar
2. Click "Add Tenant" button
3. Fill in form:
   - **Tenant ID:** `test-academy`
   - **Name:** `Test Academy`
   - **Admin Email:** `admin@test.com`
   - **Password:** `Test123!@#`
   - **Isolated Database:** ? (unchecked for faster testing)
4. Click "Save"

### Expected Results:
? **Immediate (< 1 second):**
- Modal closes
- Toast: "Tenant creation initiated. Provisioning will happen in the background."
- Tenant appears in list with **? Pending** badge

? **After 5-10 seconds:**
- Toast: "Tenant 'Test Academy' has been successfully created and provisioned."
- Status badge updates to **? Active** (green)
- Tenant list auto-refreshes

### Console Output:
```
?? Tenant Created: {
  type: "success",
  message: "Tenant 'Test Academy' has been successfully created...",
  tenantId: "test-academy",
  tenantName: "Test Academy"
}
```

---

## 4?? Test Update Tenant (1 minute)

### Steps:
1. Click "Edit" on the test-academy tenant
2. Change name to: `Updated Test Academy`
3. Click "Save"

### Expected Results:
? **Immediate:**
- Modal closes
- Toast: "Tenant update initiated..."
- Status badge shows **?? Updating...** (yellow with spinner)

? **After 2-3 seconds:**
- Toast: "Tenant 'Updated Test Academy' has been successfully updated."
- Status badge returns to **? Active**
- Updated name displayed

---

## 5?? Test Failure Scenario (2 minutes)

### Steps:
1. Click "Add Tenant"
2. Use **same Tenant ID:** `test-academy` (duplicate)
3. Fill in other fields
4. Click "Save"

### Expected Results:
? **Immediate:**
- Error toast: "Tenant already exists"
- Modal stays open
- No tenant added to list

---

## 6?? Verify Hangfire Dashboard (2 minutes)

### Steps:
1. Navigate to: `https://localhost:7298/`
2. Click "Open Hangfire Dashboard ?"
3. Click "Jobs" ? "Succeeded"

### Expected:
? See your jobs listed:
- `ProvisionTenantJob.ExecuteAsync` (for tenant creation)
- `UpdateTenantJob.ExecuteAsync` (for tenant update)

### Click on a job to see:
- Parameters (tenantId, request, userId)
- Execution time
- Logs (if any)
- Retry information

---

## 7?? Verify Database (1 minute)

### Query:
```sql
SELECT id, name, status, provisioning_error, created_on
FROM tenants
WHERE id = 'test-academy';
```

### Expected Result:
| id | name | status | provisioning_error | created_on |
|----|------|--------|-------------------|------------|
| test-academy | Updated Test Academy | 2 (Active) | null | 2024-12-09... |

**Status Values:**
- 0 = Pending
- 1 = Provisioning
- 2 = Active
- 3 = Failed
- 4 = Updating

---

## 8?? Test Multi-Tab Sync (2 minutes)

### Steps:
1. Open application in **two browser tabs** (same user)
2. Create a new tenant from **Tab 1**
3. Watch **Tab 2**

### Expected Results:
? **Tab 2 automatically:**
- Receives SignalR notification
- Shows toast message
- Updates tenant list (without manual refresh)

---

## ?? Troubleshooting

### ? SignalR not connecting
**Check:**
- WebApi is running (`https://localhost:7250`)
- Console shows: `? SignalR Connected`
- Network tab shows WebSocket connection

**Fix:**
- Refresh browser
- Check JWT token is present
- Restart WebApi service

### ? No toast notifications
**Check:**
- Console shows event received
- ToastContainer in App.tsx

**Fix:**
- Check browser console for errors
- Verify toast theme matches app theme

### ? Tenant list not refreshing
**Check:**
- Network tab for API call
- MobX observability

**Fix:**
- Manually refresh page
- Check console for errors

### ? Job fails in Hangfire
**Check:**
- Hangfire dashboard job details
- BackgroundJobs console logs
- Database connection strings

**Fix:**
- Verify database is running
- Check user credentials valid
- Retry job from Hangfire dashboard

---

## ?? Success Criteria

After completing all tests, you should have:

? **Frontend:**
- [x] SignalR connected successfully
- [x] Tenant created with real-time notification
- [x] Tenant updated with real-time notification
- [x] Status badges displaying correctly
- [x] Multi-tab synchronization working

? **Backend:**
- [x] Jobs visible in Hangfire dashboard
- [x] Jobs completed successfully
- [x] Notifications sent via SignalR
- [x] Database updated correctly

? **User Experience:**
- [x] UI remains responsive (no freezing)
- [x] Immediate feedback (< 1 second response)
- [x] Real-time notifications received
- [x] Automatic list refresh

---

## ?? Test Report Template

```
## Test Results - Tenant Background Jobs + SignalR

**Date:** _______________
**Tester:** _______________
**Environment:** Development

### Test 1: Create Tenant
- [ ] Modal closes immediately
- [ ] Pending status shows
- [ ] Success notification received
- [ ] Status updates to Active
- [ ] Result: ? Pass ? Fail

### Test 2: Update Tenant
- [ ] Modal closes immediately
- [ ] Updating status shows
- [ ] Success notification received
- [ ] Changes reflected
- [ ] Result: ? Pass ? Fail

### Test 3: Duplicate Tenant
- [ ] Error message shows
- [ ] Modal stays open
- [ ] No tenant added
- [ ] Result: ? Pass ? Fail

### Test 4: SignalR Connection
- [ ] Connection established
- [ ] Notifications received
- [ ] Auto-refresh works
- [ ] Result: ? Pass ? Fail

### Test 5: Multi-Tab Sync
- [ ] Both tabs notified
- [ ] Both tabs refresh
- [ ] Result: ? Pass ? Fail

### Overall Status: ? All Pass ? Some Fail

**Notes:**
_______________________________________
_______________________________________
_______________________________________
```

---

## ?? Next Steps

After successful testing:

1. ? Mark all tests as passed
2. ? Fill out test report
3. ? Share results with team
4. ? Proceed to staging deployment

**Questions?** Check:
- [Full Testing Guide](./Testing_TenantBackgroundJobs.md)
- [Implementation Summary](./PROJECT_COMPLETE_TenantBackgroundJobs.md)
- [Troubleshooting Section](#-troubleshooting)

---

**Estimated Total Time:** 15-30 minutes  
**Difficulty:** Easy  
**Prerequisites:** Services running, root user access
