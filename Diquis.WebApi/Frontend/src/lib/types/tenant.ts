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

// Create Tenant
export interface CreateTenantRequest {
    id: string;
    name: string;
    adminEmail: string;
    password: string;
    hasIsolatedDatabase?: boolean;
}

// Update Tenant
export interface UpdateTenantRequest {
    name: string;
    isActive: boolean;
}

