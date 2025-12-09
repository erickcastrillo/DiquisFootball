import React from 'react';
import { Badge, Spinner } from 'react-bootstrap';
import type { Tenant } from 'lib/types';

interface TenantStatusBadgeProps {
    status: Tenant['status'];
}

const TenantStatusBadge: React.FC<TenantStatusBadgeProps> = ({ status }) => {
    const getStatusConfig = (status: Tenant['status']) => {
        const configs = {
            Pending: {
                variant: 'secondary',
                label: 'Pending',
                icon: '?',
                showSpinner: false
            },
            Provisioning: {
                variant: 'info',
                label: 'Provisioning...',
                icon: null,
                showSpinner: true
            },
            Active: {
                variant: 'success',
                label: 'Active',
                icon: '?',
                showSpinner: false
            },
            Failed: {
                variant: 'danger',
                label: 'Failed',
                icon: '?',
                showSpinner: false
            },
            Updating: {
                variant: 'warning',
                label: 'Updating...',
                icon: null,
                showSpinner: true
            }
        };

        return configs[status];
    };

    const config = getStatusConfig(status);

    return (
        <Badge bg={config.variant} className="d-flex align-items-center gap-1" style={{ width: 'fit-content' }}>
            {config.showSpinner && <Spinner animation="border" size="sm" style={{ width: '12px', height: '12px' }} />}
            {config.icon && <span>{config.icon}</span>}
            <span>{config.label}</span>
        </Badge>
    );
};

export default TenantStatusBadge;
