import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from 'react-toastify';
import { store } from 'stores/store';

interface TenantNotification {
    type: 'success' | 'error';
    message: string;
    tenantId?: string;
    tenantName?: string;
    timestamp: string;
}

export const useSignalR = () => {
    const connectionRef = useRef<signalR.HubConnection | null>(null);

    useEffect(() => {
        const token = store.authStore.token;
        
        if (!token) {
            return;
        }

        const baseUrl = import.meta.env.VITE_API_URL.replace('/api', '');
        const hubUrl = `${baseUrl}/hubs/notifications`;

        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: retryContext => {
                    if (retryContext.previousRetryCount === 0) return 0;
                    if (retryContext.previousRetryCount === 1) return 2000;
                    if (retryContext.previousRetryCount === 2) return 10000;
                    return 30000;
                }
            })
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connectionRef.current = newConnection;

        // Register event handlers
        newConnection.on('Connected', (message: string) => {
            console.log('SignalR:', message);
        });

        newConnection.on('TenantCreated', (data: any) => {
            console.log('Tenant created:', data);
            toast.success(data.message || 'Tenant created successfully!', {
                autoClose: 5000,
            });
            store.tenantsStore.loadTenants();
        });

        newConnection.on('TenantCreationFailed', (data: any) => {
            console.error('Tenant creation failed:', data);
            toast.error(data.message || 'Failed to create tenant', {
                autoClose: 10000,
            });
            store.tenantsStore.loadTenants();
        });

        newConnection.on('TenantUpdated', (data: any) => {
            console.log('Tenant updated:', data);
            toast.success(data.message || 'Tenant updated successfully!', {
                autoClose: 5000,
            });
            store.tenantsStore.loadTenants();
        });

        newConnection.on('TenantUpdateFailed', (data: any) => {
            console.error('Tenant update failed:', data);
            toast.error(data.message || 'Failed to update tenant', {
                autoClose: 10000,
            });
            store.tenantsStore.loadTenants();
        });

        // Connection lifecycle events
        newConnection.onreconnecting(() => {
            console.warn('SignalR reconnecting...');
        });

        newConnection.onreconnected((connectionId) => {
            console.log('SignalR reconnected:', connectionId);
        });

        newConnection.onclose((error) => {
            if (error) {
                console.error('SignalR connection closed with error:', error);
            }
        });

        // Start connection
        newConnection.start()
            .then(() => {
                console.log('SignalR connected');
            })
            .catch(err => {
                console.error('SignalR connection error:', err);
            });

        // Cleanup on unmount
        return () => {
            newConnection.stop();
        };
    }, [store.authStore.token]);

    return connectionRef.current;
};
