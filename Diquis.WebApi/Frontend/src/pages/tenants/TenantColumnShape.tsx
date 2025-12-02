import clsx from 'clsx';
import { useModal } from 'hooks';
import { TableOptions } from '@tanstack/react-table';

import EditTenantModal from './EditTenantModal';
import type { Tenant } from 'lib/types';

// columns for the tenants table
const TenantColumnShape: TableOptions<Tenant>['columns'] = [
  {
    header: 'Id',
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
    header: 'Name',
    enableSorting: true,
    accessorKey: 'name'
  },
  {
    header: 'Active',
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
        {getValue()?.toString() == 'true' ? 'Yes' : 'No'}
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
              Edit
            </button>
          ) : (
            'N/A(You)'
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
