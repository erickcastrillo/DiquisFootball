import { Badge } from 'react-bootstrap';
import clsx from 'clsx';
import { TableOptions } from '@tanstack/react-table';
import { TFunction } from 'i18next';

import EditUserModal from './EditUserModal';
import { useStore } from 'stores/store';
import { Avatar } from 'components/ui';
import { getInitials } from 'utils';
import type { User } from 'lib/types';
import { useModal } from 'hooks';

const UserColumnShape = ({ t }: { t: TFunction }): TableOptions<User>['columns'] => [
  {
    header: t('users.columns.name'),
    enableSorting: true,
    accessorFn: (row) => row.firstName + ' ' + row.lastName,
    cell: ({ row }) => {
      const { firstName, lastName, imageUrl } = row.original;

      return (
        <div className="table-profile-component">
          <Avatar src={imageUrl} text={getInitials(firstName, lastName)} />
          <strong>{firstName + ' ' + lastName}</strong>
        </div>
      );
    }
  },
  {
    header: t('users.columns.role'),
    enableSorting: true,
    accessorKey: 'roleId',
    enableGlobalFilter: false,
    cell: ({ getValue }) => {
      const badgeBgMap = {
        root: 'success',
        admin: 'success',
        editor: 'info',
        basic: 'info'
      };

      return (
        <h4>
          <Badge
            bg={
              badgeBgMap[getValue()?.toLowerCase() as keyof typeof badgeBgMap]
            }
            className="text-light text-capitalize"
          >
            {getValue()}
          </Badge>
        </h4>
      );
    }
  },
  {
    header: t('users.columns.email'),
    enableSorting: true,
    accessorKey: 'email'
  },
  {
    header: t('users.columns.active'),
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

      const { accountStore } = useStore();
      const { currentUser } = accountStore;
      const { show, onShow, onHide } = useModal();

      return (
        <>
          {id != currentUser?.id ? (
            <button
              className="btn btn-soft-primary rounded-pill"
              onClick={onShow}
            >
              {t('users.columns.edit')}
            </button>
          ) : (
            t('users.columns.naYou')
          )}

          {show && (
            <EditUserModal show={show} onHide={onHide} data={row.original} />
          )}
        </>
      );
    }
  }
];

export default UserColumnShape;
