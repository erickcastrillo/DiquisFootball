import clsx from 'clsx';
import { useModal } from 'hooks';
import { TableOptions } from '@tanstack/react-table';
import { TFunction } from 'i18next';
import { Alert } from 'react-bootstrap';

import EditTenantModal from './EditTenantModal';
import TenantStatusBadge from 'components/TenantStatusBadge';
import type { Tenant } from 'lib/types';

const TenantColumnShape = ({ t }: { t: TFunction }): TableOptions<Tenant>['columns'] => [
  {
    header: t('tenants.columns.id'),
    accessorKey: 'id',
    enableSorting: true,
    cell: ({ row }) => {
      const { id } = row.original;
      return (
        <div className="table-profile-component">
          <strong>{id}</strong>
        </div>
      );
    }
  },
  {
    header: t('tenants.columns.name'),
    enableSorting: true,
    accessorKey: 'name',
    cell: ({ row }) => {
      const { name, provisioningError } = row.original;
      return (
        <div>
          <div>{name}</div>
          {provisioningError && (
            <Alert variant="danger" className="mt-1 mb-0 p-1" style={{ fontSize: '0.75rem' }}>
              <strong>Error:</strong> {provisioningError}
            </Alert>
          )}
        </div>
      );
    }
  },
  {
    header: t('tenants.columns.status'),
    enableSorting: true,
    accessorKey: 'status',
    cell: ({ row }) => {
      const { status } = row.original;
      return <TenantStatusBadge status={status} />;
    }
  },
  {
    header: t('tenants.columns.active'),
    enableSorting: true,
    accessorKey: 'isActive',
    enableGlobalFilter: false,
    cell: ({ getValue }) => (
      <strong
        className={clsx(
          getValue()?.toString().toLowerCase() === 'true'
            ? 'text-success'
            : 'text-danger',
          'text-capitalize'
        )}
      >
        {getValue()?.toString() == 'true' ? t('common.yes') : t('common.no')}
      </strong>
    )
  },
  {
    header: '',
    accessorKey: 'edit',
    cell: ({ row }) => {
      const { id, status } = row.original;
      const { show, onShow, onHide } = useModal();
      
      // Disable edit button while provisioning/updating
      const isProcessing = status === 'Provisioning' || status === 'Updating' || status === 'Pending';

      return (
        <>
          {id != 'root' ? (
            <button
              className="btn btn-soft-primary rounded-pill"
              onClick={onShow}
              disabled={isProcessing}
            >
              {t('tenants.columns.edit')}
            </button>
          ) : (
            t('tenants.columns.naYou')
          )}
          {show && (
            <EditTenantModal show={show} onHide={onHide} data={row.original} />
          )}
        </>
      );
    }
  }
];

export default TenantColumnShape;
