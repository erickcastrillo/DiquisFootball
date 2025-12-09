# ?? Tenant Background Jobs - Developer Handoff Checklist

**Feature:** Asynchronous Tenant Provisioning with SignalR Notifications  
**Status:** ? Backend Complete | ? Frontend Pending  
**Date:** 2024-12-09

---

## ? Backend Implementation (COMPLETE)

### Phase 1: Domain & Database ?
- [x] Created `ProvisioningStatus` enum
- [x] Updated `Tenant` entity with status fields
- [x] Created and applied database migration
- [x] Build successful with no errors

### Phase 2: SignalR Infrastructure ?
- [x] Created `INotificationService` interface
- [x] Implemented `SignalRNotificationService`
- [x] Created `NotificationHub` with authorization
- [x] Registered SignalR services in DI
- [x] Mapped hub endpoint to `/hubs/notifications`

### Phase 3: Background Jobs ?
- [x] Created `ProvisionTenantJob` with full logic
- [x] Created `UpdateTenantJob` with validation
- [x] Registered jobs in both WebApi and BackgroundJobs projects
- [x] Jobs send SignalR notifications on completion
- [x] Comprehensive error handling and logging

### Phase 4: Service Layer ?
- [x] Updated `ITenantManagementService` interface
- [x] Refactored `TenantManagementService` to enqueue jobs
- [x] Changed return types to `Response<string>`
- [x] Added `initiatingUserId` parameter

### Phase 5: API Layer ?
- [x] Updated `TenantsController` endpoints
- [x] Changed status codes to 202 Accepted
- [x] Added user ID extraction from JWT
- [x] Updated XML documentation

### Phase 6: Testing ?
- [x] Build successful
- [x] All existing tests pass
- [x] Ready for manual testing

---

## ? Frontend Integration (PENDING)

### Prerequisites
- [ ] Backend services running (WebApi + BackgroundJobs)
- [ ] Frontend development environment set up
- [ ] Access to frontend codebase

### Step 1: Install Dependencies ?
```bash
cd Diquis.WebApi/Frontend
npm install @microsoft/signalr
```

**Verification:**
- [ ] Package appears in `package.json`
- [ ] No installation errors

### Step 2: Create SignalR Hook ?
**File:** `src/hooks/useSignalR.ts`

**Reference:** See complete implementation in:
- `docs/Technical documentation/Frontend_SignalR_Integration_Guide.md`

**Implementation checklist:**
- [ ] Import SignalR and React hooks
- [ ] Get auth token from store
- [ ] Create connection with token
- [ ] Configure automatic reconnect
- [ ] Add connection lifecycle logging
- [ ] Listen for 4 events: TenantCreated, TenantCreationFailed, TenantUpdated, TenantUpdateFailed
- [ ] Display toast notifications on events
- [ ] Refresh tenant list on events
- [ ] Cleanup on unmount

**Testing:**
- [ ] Hook compiles without errors
- [ ] Can be imported in components

### Step 3: Update Type Definitions ?
**File:** `src/lib/types/tenant.ts` (or similar)

**Add to Tenant interface:**
```typescript
status: 'Pending' | 'Provisioning' | 'Active' | 'Failed' | 'Updating';
provisioningError?: string;
lastProvisioningAttempt?: string;
```

**Verification:**
- [ ] TypeScript compiles without errors
- [ ] Existing tenant code doesn't break

### Step 4: Initialize SignalR Connection ?
**Choose one location:**

**Option A: Main Layout Component**
```typescript
import { useSignalR } from 'hooks/useSignalR';

export const Layout = ({ children }) => {
  useSignalR(); // Initialize here
  return <div>{children}</div>;
};
```

**Option B: Root App Component**
```typescript
import { useSignalR } from 'hooks/useSignalR';

function App() {
  useSignalR(); // Initialize here
  return <Router>...</Router>;
}
```

**Verification:**
- [ ] Browser console shows "?? SignalR Connected"
- [ ] Network tab shows WebSocket connection
- [ ] No console errors

### Step 5: Update Tenant List UI ?
**File:** `src/features/tenants/TenantList.tsx` (or similar)

**Add status badge component:**
```typescript
const getStatusBadge = (status: Tenant['status']) => {
  // See implementation in Frontend_SignalR_Integration_Guide.md
};
```

**Update table/list:**
```tsx
<td>{getStatusBadge(tenant.status)}</td>
```

**Show errors:**
```tsx
{tenant.status === 'Failed' && tenant.provisioningError && (
  <Alert variant="danger">
    <strong>Error:</strong> {tenant.provisioningError}
  </Alert>
)}
```

**Verification:**
- [ ] Status badges display correctly
- [ ] Error messages show for failed tenants
- [ ] UI updates when status changes

### Step 6: Handle Loading States ?
**Optional but recommended:**

```tsx
const isProcessing = tenant.status === 'Pending' || 
                     tenant.status === 'Provisioning' ||
                     tenant.status === 'Updating';

{isProcessing && (
  <Spinner animation="border" size="sm" />
)}
```

**Verification:**
- [ ] Loading indicators show during processing
- [ ] Indicators disappear when complete

---

## ?? End-to-End Testing Checklist

### Test 1: Create Tenant (Success) ?
- [ ] POST to `/api/tenants` with valid data
- [ ] Response is 202 Accepted
- [ ] Hangfire shows job enqueued
- [ ] Toast notification appears after ~5 seconds
- [ ] Tenant list refreshes automatically
- [ ] Tenant shows "Active" status
- [ ] Admin user can login to new tenant

### Test 2: Create Tenant (Duplicate) ?
- [ ] POST with existing tenant ID
- [ ] Response is 400 Bad Request
- [ ] Error message is clear
- [ ] No job enqueued
- [ ] No database changes

### Test 3: Create Tenant (Invalid Data) ?
- [ ] POST with missing required fields
- [ ] Response is 400 Bad Request
- [ ] Validation errors returned
- [ ] No job enqueued

### Test 4: Update Tenant (Success) ?
- [ ] PUT to `/api/tenants/{id}` with valid data
- [ ] Response is 202 Accepted
- [ ] Toast notification appears
- [ ] Tenant list refreshes
- [ ] Changes reflected in database

### Test 5: Update Root Tenant (Failure) ?
- [ ] PUT to `/api/tenants/root`
- [ ] Response is 400 Bad Request
- [ ] Error message: "Cannot edit root tenant"
- [ ] No changes made

### Test 6: SignalR Connection ?
- [ ] Login to application
- [ ] Browser console shows SignalR connected
- [ ] Network tab shows WebSocket open
- [ ] Create tenant from different browser tab
- [ ] Notification appears in both tabs
- [ ] Both tabs refresh simultaneously

### Test 7: Failure Scenario ?
- [ ] Create tenant with invalid database config
- [ ] Job fails in Hangfire
- [ ] Error toast appears
- [ ] Tenant status = "Failed"
- [ ] Error message stored in database
- [ ] Can view error in UI

### Test 8: Multiple Users ?
- [ ] User A creates tenant
- [ ] User B logged in simultaneously
- [ ] Only User A receives notification
- [ ] User B doesn't get notification
- [ ] Both see updated list on refresh

---

## ?? Documentation Review Checklist

### For Developers ?
- [ ] Read: `IMPLEMENTATION_COMPLETE_TenantBackgroundJobs.md`
- [ ] Read: `Frontend_SignalR_Integration_Guide.md`
- [ ] Read: `BackgroundJobs_Hangfire_TechnicalGuide.md`
- [ ] Understand: Clean Architecture layers
- [ ] Know how to: Enqueue background jobs
- [ ] Know how to: Send SignalR notifications

### For QA ?
- [ ] Read: `Testing_TenantBackgroundJobs.md`
- [ ] Know how to: Access Hangfire dashboard
- [ ] Know how to: Monitor job execution
- [ ] Know how to: Verify database changes
- [ ] Know how to: Test failure scenarios

### For Frontend Developers ?
- [ ] Read: `Frontend_SignalR_Integration_Guide.md`
- [ ] Know how to: Connect to SignalR hub
- [ ] Know how to: Listen for events
- [ ] Know how to: Display notifications
- [ ] Know how to: Handle errors

---

## ?? Deployment Checklist

### Development Environment ?
- [ ] Both services start successfully
- [ ] Database migrations applied
- [ ] Hangfire dashboard accessible
- [ ] SignalR hub endpoint responding
- [ ] Test jobs can be enqueued

### Staging Environment ?
- [ ] Deploy WebApi project
- [ ] Deploy BackgroundJobs worker
- [ ] Run database migrations
- [ ] Configure connection strings
- [ ] Test create/update tenant flows
- [ ] Verify SignalR works across servers
- [ ] Monitor logs for errors

### Production Environment ?
- [ ] Deploy WebApi to production
- [ ] Deploy BackgroundJobs worker
- [ ] Run database migrations
- [ ] Configure production connection strings
- [ ] Set up monitoring/alerting
- [ ] Test with real data
- [ ] Monitor Hangfire dashboard
- [ ] Verify job processing capacity

---

## ?? Known Limitations & Future Enhancements

### Current Limitations
- ? No retry UI for failed provisioning (manual via Hangfire dashboard)
- ? No progress updates during provisioning (only start/end notifications)
- ? No tenant deletion implemented yet
- ? SignalR only notifies user who initiated the action (not all admins)

### Future Enhancements
- [ ] Add retry endpoint to API
- [ ] Send progress updates (e.g., "Creating database...", "Running migrations...")
- [ ] Implement tenant deletion with background job
- [ ] Broadcast notifications to all admins of a tenant
- [ ] Add email notifications as fallback
- [ ] Implement job cancellation
- [ ] Add job priority levels

---

## ?? Quick Links

### Documentation
- [Complete Implementation Summary](../Implementation%20plans/IMPLEMENTATION_COMPLETE_TenantBackgroundJobs.md)
- [Frontend Integration Guide](../Technical%20documentation/Frontend_SignalR_Integration_Guide.md)
- [Testing Guide](../Quick%20Reference/Testing_TenantBackgroundJobs.md)
- [Technical Guide](../Technical%20documentation/BackgroundJobs_Hangfire_TechnicalGuide.md)

### Dashboards
- Swagger UI: `https://localhost:7250/swagger`
- Hangfire Dashboard: `https://localhost:7298/hangfire`
- WebApi: `https://localhost:7250`
- BackgroundJobs: `https://localhost:7298`

### Key Files
- Backend Jobs: `Diquis.Infrastructure/BackgroundJobs/`
- SignalR Hub: `Diquis.Infrastructure/Hubs/NotificationHub.cs`
- Tenant Service: `Diquis.Infrastructure/Multitenancy/TenantManagementService.cs`
- API Controller: `Diquis.WebApi/Controllers/TenantsController.cs`

---

## ? Sign-off

### Backend Developer
- [x] Implementation complete
- [x] Build successful
- [x] Self-tested locally
- [x] Documentation updated
- [x] Ready for frontend integration

**Signed:** _________________________  
**Date:** _________________________

### Frontend Developer
- [ ] Frontend integration complete
- [ ] SignalR connected successfully
- [ ] UI updated with status badges
- [ ] Notifications working
- [ ] End-to-end tested

**Signed:** _________________________  
**Date:** _________________________

### QA Tester
- [ ] All test scenarios passed
- [ ] Edge cases tested
- [ ] Performance acceptable
- [ ] No critical bugs found
- [ ] Ready for deployment

**Signed:** _________________________  
**Date:** _________________________

---

**Current Status:** ? Backend Complete | ? Frontend Pending  
**Next Action:** Frontend developer to begin Step 1 of Frontend Integration

**Questions?** Check documentation or ask in team chat.
