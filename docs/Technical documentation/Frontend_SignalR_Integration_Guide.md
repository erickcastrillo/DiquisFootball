# Frontend Integration Guide: Tenant SignalR Notifications

## Quick Start

### 1. Install SignalR Client
```bash
cd Diquis.WebApi/Frontend
npm install @microsoft/signalr
```

### 2. Create SignalR Hook

**File:** `src/hooks/useSignalR.ts`

```typescript
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from 'react-toastify';
import { store } from 'stores/store';

export const useSignalR = () => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

    useEffect(() => {
        const token = store.authStore.token;
        if (!token) return;

        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${import.meta.env.VITE_API_URL.replace('/api', '')}/hubs/notifications`, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (!connection) return;

        connection.start()
            .then(() => console.log('? SignalR Connected'))
            .catch(err => console.error('? SignalR Connection Error:', err));

        // Tenant Created (Success)
        connection.on('TenantCreated', (data) => {
            toast.success(data.message);
            store.tenantsStore.loadTenants(); // Refresh tenant list
        });

        // Tenant Creation Failed
        connection.on('TenantCreationFailed', (data) => {
            toast.error(data.message);
            store.tenantsStore.loadTenants(); // Refresh to show failed status
        });

        // Tenant Updated (Success)
        connection.on('TenantUpdated', (data) => {
            toast.success(data.message);
            store.tenantsStore.loadTenants(); // Refresh tenant list
        });

        // Tenant Update Failed
        connection.on('TenantUpdateFailed', (data) => {
            toast.error(data.message);
            store.tenantsStore.loadTenants(); // Refresh to show error
        });

        return () => {
            connection.stop();
        };
    }, [connection]);

    return connection;
};
```

### 3. Update Tenant Types

**File:** `src/lib/types/tenant.ts`

```typescript
// Tenant
export interface Tenant {
    id: string;
    name: string;
    isActive: boolean;
    createdOn: string;
    status: 'Pending' | 'Provisioning' | 'Active' | 'Failed' | 'Updating';
    provisioningError?: string;
    lastProvisioningAttempt?: string;
}

// Create Tenant (no changes needed)
export interface CreateTenantRequest {
    id: string;
    name: string;
    adminEmail: string;
    password: string;
    hasIsolatedDatabase?: boolean;
}

// Update Tenant (no changes needed)
export interface UpdateTenantRequest {
    name: string;
    isActive: boolean;
}
```

### 4. Use Hook in Layout

**Option A: In Main Layout Component**
```typescript
// src/components/PageLayout.tsx (or your main layout)
import { useSignalR } from 'hooks/useSignalR';

export const PageLayout = ({ children, title }: Props) => {
    useSignalR(); // Initialize SignalR connection

    return (
        <div>
            <h1>{title}</h1>
            {children}
        </div>
    );
};
```

**Option B: In Root App Component**
```typescript
// src/App.tsx
import { useSignalR } from 'hooks/useSignalR';

function App() {
    useSignalR(); // Initialize once at app level

    return (
        <Router>
            {/* Your routes */}
        </Router>
    );
}
```

### 5. Update Tenant List UI

**File:** `src/features/tenants/TenantList.tsx` (or wherever you display tenants)

```typescript
import { Badge } from 'react-bootstrap';

const getStatusBadge = (status: Tenant['status']) => {
    const variants = {
        Pending: 'secondary',
        Provisioning: 'info',
        Active: 'success',
        Failed: 'danger',
        Updating: 'warning'
    };

    const labels = {
        Pending: '? Pending',
        Provisioning: '?? Provisioning...',
        Active: '? Active',
        Failed: '? Failed',
        Updating: '?? Updating...'
    };

    return (
        <Badge bg={variants[status]}>
            {labels[status]}
        </Badge>
    );
};

// In your table/list component:
<td>{getStatusBadge(tenant.status)}</td>
```

### 6. Show Provisioning Error (if exists)

```typescript
{tenant.status === 'Failed' && tenant.provisioningError && (
    <div className="alert alert-danger mt-2">
        <strong>Error:</strong> {tenant.provisioningError}
    </div>
)}
```

## SignalR Event Payload Structure

### TenantCreated
```typescript
{
    type: "success",
    message: "Tenant 'Demo Academy' has been successfully created and provisioned.",
    tenantId: "demo-academy",
    tenantName: "Demo Academy",
    timestamp: "2024-12-09T05:45:23Z"
}
```

### TenantCreationFailed
```typescript
{
    type: "error",
    message: "Tenant creation failed: Database migration error",
    timestamp: "2024-12-09T05:45:23Z"
}
```

### TenantUpdated
```typescript
{
    type: "success",
    message: "Tenant 'Demo Academy' has been successfully updated.",
    tenantId: "demo-academy",
    tenantName: "Demo Academy",
    timestamp: "2024-12-09T05:45:23Z"
}
```

### TenantUpdateFailed
```typescript
{
    type: "error",
    message: "Tenant update failed: Validation error",
    tenantId: "demo-academy",
    timestamp: "2024-12-09T05:45:23Z"
}
```

## Updated API Response Format

### Create Tenant (POST /api/tenants)
**Status Code:** `202 Accepted`

```json
{
    "succeeded": true,
    "data": "demo-academy",
    "messages": [
        "Tenant creation initiated. The tenant will be provisioned in the background."
    ]
}
```

### Update Tenant (PUT /api/tenants/{id})
**Status Code:** `202 Accepted`

```json
{
    "succeeded": true,
    "data": "demo-academy",
    "messages": [
        "Tenant update initiated. The tenant will be updated in the background."
    ]
}
```

## Testing Checklist

### Backend Testing
- [ ] Start WebApi: `dotnet run --project Diquis.WebApi`
- [ ] Start BackgroundJobs: `dotnet run --project Diquis.BackgroundJobs`
- [ ] Open Hangfire Dashboard: https://localhost:7298/hangfire
- [ ] Verify SignalR hub endpoint is mapped: https://localhost:7250/hubs/notifications

### Frontend Testing
- [ ] Install @microsoft/signalr package
- [ ] Create useSignalR hook
- [ ] Update tenant types
- [ ] Use hook in layout/app component
- [ ] Open browser console - verify "? SignalR Connected" message
- [ ] Create a tenant - should see:
  - [ ] Immediate success response (202 Accepted)
  - [ ] Toast notification when provisioning completes
  - [ ] Tenant list auto-refreshes
  - [ ] Status badge updates to "Active"
- [ ] Test failure scenario (e.g., duplicate tenant ID):
  - [ ] Should see error toast
  - [ ] Status shows "Failed"
  - [ ] Error message displayed

## Troubleshooting

### SignalR Not Connecting
1. Check browser console for errors
2. Verify VITE_API_URL environment variable
3. Check that token is present: `store.authStore.token`
4. Verify WebApi is running and hub is mapped

### Notifications Not Appearing
1. Check browser console for `connection.on()` logs
2. Verify user is authenticated (JWT token contains user ID)
3. Check Hangfire dashboard - is job completing successfully?
4. Look at WebApi console logs for notification sending

### Status Not Updating
1. Check that tenant list is refreshing after notification
2. Verify backend is updating tenant status correctly
3. Check database - confirm Status column has correct value

## Common Issues

### Issue: "SignalR connection failed with status code 401"
**Solution:** Ensure JWT token is being passed correctly in `accessTokenFactory`

### Issue: Notifications not received
**Solution:** 
- Verify user ID in JWT matches the ID passed to job
- Check that `Context.UserIdentifier` is working in the hub
- Ensure ASP.NET Core Identity is properly configured

### Issue: Tenant list not refreshing
**Solution:** 
- Confirm `store.tenantsStore.loadTenants()` is being called in event handlers
- Check MobX store is properly updating observable data

## Additional Enhancements (Optional)

### Show Loading State During Provisioning
```typescript
const isPending = tenant.status === 'Pending' || tenant.status === 'Provisioning';

{isPending && (
    <div className="d-flex align-items-center">
        <Spinner animation="border" size="sm" className="me-2" />
        <span>Provisioning in progress...</span>
    </div>
)}
```

### Retry Failed Provisioning
```typescript
const retryProvisioning = async (tenantId: string) => {
    // Make API call to retry endpoint (you'd need to implement this)
    await agent.Tenants.retry(tenantId);
    toast.info('Retry initiated...');
};
```

### Real-time Progress Updates
If you want more granular progress (e.g., "Creating database...", "Running migrations..."), you can add additional SignalR events in the backend job and listen for them in the frontend.

## Need Help?
- Check Hangfire dashboard logs: https://localhost:7298/hangfire
- Review WebApi console for SignalR connection logs
- Inspect browser Network tab for WebSocket connection
- Check browser console for JavaScript errors
