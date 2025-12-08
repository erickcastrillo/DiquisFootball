import clsx from 'clsx';
import { useModal } from 'hooks';
import { TableOptions } from '@tanstack/react-table';
import { TFunction } from 'i18next';

import EditTenantModal from './EditTenantModal';
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
    accessorKey: 'name'
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
      const { id } = row.original;
      const { show, onShow, onHide } = useModal();

      return (
        <>
          {id != 'root' ? (
            <button
              className="btn btn-soft-primary rounded-pill"
              onClick={onShow}
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
