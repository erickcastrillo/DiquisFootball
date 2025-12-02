import React from 'react';
import { OverlayTrigger, Tooltip, TooltipProps } from 'react-bootstrap';
import {
  PaginationState,
  SortingState,
  TableOptions
} from '@tanstack/react-table';

import { useStore } from 'stores/store';
import { Roles, type Product } from 'lib/types';
import ProductModal from './ProductModal';
import GeneratePdfButton from './GeneratePdfButton'; // Add this import
import { useModal } from 'hooks';

type GetProductColumnShapeOptions = {
  pagination: PaginationState;
  filteredQuery: string;
  sorting: SortingState;
};

const getProductColumnShape = ({
  pagination,
  filteredQuery,
  sorting
}: GetProductColumnShapeOptions): TableOptions<Product>['columns'] => [
  {
    header: 'Name',
    enableSorting: true,
    accessorKey: 'name',
    cell: ({ row }) => {
      return <strong className="text-capitalize text-dark">{row.original.name}</strong>;
    }
  },
  {
    header: 'Description',
    enableSorting: true,
    accessorKey: 'description'
  },
  {
    header: 'Guid',
    enableSorting: true,
    accessorKey: 'id'
  },
  {
    header: 'Actions', // Changed header to 'Actions' since we now have multiple actions
    accessorKey: 'actions', // Changed accessorKey
    cell: (row) => {
      const { accountStore } = useStore();
      const { show, onShow, onHide } = useModal();

      const renderTooltip = (props: TooltipProps) => (
        <Tooltip id="button-tooltip" className="font-14" {...props}>
          Basic user cannot use this feature
        </Tooltip>
      );

      return (
        <React.Fragment>
          <div className="d-flex gap-2 justify-content-end"> {/* Container for multiple buttons */}
            {/* Edit Button */}
            <GeneratePdfButton product={row.row.original} />
            {accountStore.currentUser?.roleId !== Roles.basic ? (
              <button
                className="btn btn-soft-primary rounded-pill"
                onClick={onShow}
              >
                Edit
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
                  Edit
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