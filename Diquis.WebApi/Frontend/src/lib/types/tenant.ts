// Tenant
export interface Tenant {
    id: string;
    name: string;
    isActive: boolean;
    createdOn: string;
}

// Create Tenant
export interface CreateTenantRequest {
    id: string;
    name: string;
    adminEmail: string;
    password: string;
}

