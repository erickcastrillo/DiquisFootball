import React from 'react';
import { OverlayTrigger, Tooltip, TooltipProps } from 'react-bootstrap';
import {
  PaginationState,
  SortingState,
  TableOptions
} from '@tanstack/react-table';
import { TFunction } from 'i18next';

import { useStore } from 'stores/store';
import { Roles, type Product } from 'lib/types';
import ProductModal from './ProductModal';
import GeneratePdfButton from './GeneratePdfButton';
import { useModal } from 'hooks';

type GetProductColumnShapeOptions = {
  pagination: PaginationState;
  filteredQuery: string;
  sorting: SortingState;
  t: TFunction;
};

const getProductColumnShape = ({
  pagination,
  filteredQuery,
  sorting,
  t,
}: GetProductColumnShapeOptions): TableOptions<Product>['columns'] => [
  {
    header: t('products.columns.name'),
    enableSorting: true,
    accessorKey: 'name',
    cell: ({ row }) => {
      return <strong className="text-capitalize text-dark">{row.original.name}</strong>;
    }
  },
  {
    header: t('products.columns.description'),
    enableSorting: true,
    accessorKey: 'description'
  },
  {
    header: t('products.columns.guid'),
    enableSorting: true,
    accessorKey: 'id'
  },
  {
    header: t('products.columns.actions'),
    accessorKey: 'actions',
    cell: (row) => {
      const { accountStore } = useStore();
      const { show, onShow, onHide } = useModal();

      const renderTooltip = (props: TooltipProps) => (
        <Tooltip id="button-tooltip" className="font-14" {...props}>
          {t('products.basicUserTooltip')}
        </Tooltip>
      );

      return (
        <React.Fragment>
          <div className="d-flex gap-2 justify-content-end">
            <GeneratePdfButton product={row.row.original} />
            {accountStore.currentUser?.roleId !== Roles.basic ? (
              <button
                className="btn btn-soft-primary rounded-pill"
                onClick={onShow}
              >
                {t('products.columns.edit')}
              </button>
            ) : (
              <OverlayTrigger
                placement="bottom-start"
                delay={{ show: 250, hide: 400 }}
                overlay={renderTooltip}
              >
                <button
                  className="btn btn-soft-primary rounded-pill"
                  aria-readonly="true"
                >
                  {t('products.columns.edit')}
                </button>
              </OverlayTrigger>
            )}
          </div>
          {show && (
            <ProductModal
              isEdit
              show={show}
              onHide={onHide}
              data={row.row.original}
              paginationState={{
                pageIndex: pagination.pageIndex + 1,
                pageSize: pagination.pageSize
              }}
              filteredQuery={filteredQuery}
              sorting={sorting}
            />
          )}
        </React.Fragment>
      );
    }
  }
];

export default getProductColumnShape;