import ScrollBar from 'simplebar-react';
import clsx from 'clsx';
import { Spinner } from 'react-bootstrap';
import {
  OnChangeFn,
  PaginationState,
  SortingState,
  TableOptions,
  flexRender,
  getFilteredRowModel,
  useReactTable
} from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';

import Pagination from 'components/Pagination';
import { SIZE_PER_PAGE_LIST } from 'constants';

// this is a custom component which receives pagination state, used with pagination api endpoints

type ServerTableProps<T> = TableOptions<T> & {
  sorting: SortingState;
  setSorting: React.Dispatch<React.SetStateAction<SortingState>>;
  pagination: PaginationState;
  setPagination: React.Dispatch<React.SetStateAction<PaginationState>>;
  pageCount: number;
  totalCount: number;
  rowClick?: (rowData: T) => void;
  hidePagination?: boolean;
  isLoading?: boolean;
};

function ServerTable<T>(props: ServerTableProps<T>) {
  const {
    sorting,
    setSorting,
    pagination,
    setPagination,
    pageCount,
    totalCount,
    hidePagination,
    rowClick,
    isLoading,
    ...tableOptions
  } = props;
  const { t } = useTranslation();

  const onPaginationChange: OnChangeFn<PaginationState> = (updaterOrValue) => {
    const newState =
      typeof updaterOrValue === 'function'
        ? updaterOrValue({
            pageIndex: pagination.pageIndex,
            pageSize: pagination.pageSize
          })
        : updaterOrValue;
    setPagination(newState);
  };

  const onSortingChange: OnChangeFn<SortingState> = (updaterOrValue) => {
    const newState =
      typeof updaterOrValue === 'function'
        ? updaterOrValue(sorting)
        : updaterOrValue;
    setSorting(newState);
  };

  const tableProps = useReactTable({
    getFilteredRowModel: getFilteredRowModel(),
    manualPagination: true,
    manualSorting: true,
    state: {
      pagination,
      sorting
    },
    pageCount,
    onPaginationChange,
    onSortingChange,
    ...tableOptions
  });

  return (
    <div className="table-responsive">
      <ScrollBar>
        <table
          className={clsx(
            'table table-striped table-centered w-100 nowrap dataTable no-footer'
          )}
        >
          <thead>
            {tableProps.getHeaderGroups().map((headerGroup) => (
              <tr key={headerGroup.id}>
                {headerGroup.headers.map(({ column, getContext }) => {
                  return (
                    <th
                      key={column.id}
                      onClick={column.getToggleSortingHandler()}
                      className={clsx({
                        sorting_desc: column.getIsSorted() === 'desc',
                        sorting_asc: column.getIsSorted() === 'asc',
                        sorting: column.columnDef.enableSorting === true,
                        'cursor-pointer':
                          column.columnDef.enableSorting === true
                      })}
                    >
                      {flexRender(column.columnDef.header, getContext())}
                    </th>
                  );
                })}
              </tr>
            ))}
          </thead>

          {!isLoading && (
            <tbody>
              {tableProps.getRowModel().rows.length === 0 ? (
                <tr>
                  <td colSpan={tableProps.getAllColumns().length} className="text-center">
                    {t('tables.noResults')}
                  </td>
                </tr>
              ) : (
                tableProps.getRowModel().rows.map((row) => {
                  return (
                    <tr
                      key={row.id}
                      onClick={
                        rowClick ? () => rowClick?.(row.original) : undefined
                      }
                    >
                      {row.getVisibleCells().map((cell) => (
                        <td key={cell.id}>
                          {flexRender(
                            cell.column.columnDef.cell,
                            cell.getContext()
                          )}
                        </td>
                      ))}
                    </tr>
                  );
                })
              )}
            </tbody>
          )}
        </table>
        {isLoading && (
          <div className="my-4 w-100 d-flex justify-content-center">
            <Spinner />
          </div>
        )}
      </ScrollBar>
      {!hidePagination && (
        <Pagination
          queryPageCount={tableProps.getPageCount()}
          queryPageSize={tableProps.getState().pagination.pageSize}
          queryPageIndex={tableProps.getState().pagination.pageIndex}
          totalCount={totalCount}
          isLoading={isLoading === undefined ? true : isLoading}
          goToPage={tableProps.setPageIndex}
          setPageSize={tableProps.setPageSize}
          sizePerPageList={SIZE_PER_PAGE_LIST}
        />
      )}
    </div>
  );
}

export default ServerTable;
