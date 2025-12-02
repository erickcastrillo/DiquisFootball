import { useState } from 'react';
import {
  SortingState,
  TableOptions,
  flexRender,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  useReactTable
} from '@tanstack/react-table';
import ScrollBar from 'simplebar-react';
import clsx from 'clsx';
import { Spinner } from 'react-bootstrap';
import { observer } from 'mobx-react-lite';

import Pagination from 'components/Pagination';
import { SIZE_PER_PAGE_LIST } from 'constants';
import { useStore } from 'stores/store';
import TableFilters from 'components/tables/ClientTable/TableFilters';

type ClientTableProps<T> = TableOptions<T> & {
  filterFieldPlaceholder: string;
  rowClick?: (rowData: T) => void;
  hidePagination?: boolean;
  showFooter?: boolean;
  isLoading?: boolean;
};

function ClientTable<T>(props: ClientTableProps<T>) {
  const {
    filterFieldPlaceholder,
    isLoading,
    rowClick,
    hidePagination,
    showFooter,
    ...tableOptions
  } = props;
  const { layoutStore } = useStore();
  const [globalFilter, setGlobalFilter] = useState('');
  const [sorting, setSorting] = useState<SortingState>([]);

  const tableProps = useReactTable({
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    state: {
      globalFilter,
      sorting
    },
    onGlobalFilterChange: (val) => {
      setGlobalFilter(val);
    },
    onSortingChange: setSorting,
    initialState: {
      pagination: {
        pageIndex: 0,
        pageSize: layoutStore.pageSize
      }
    },
    ...tableOptions
  });

  return (
    <div>
      <TableFilters
        globalFilter={tableProps.getState().globalFilter}
        setGlobalFilter={tableProps.setGlobalFilter}
        inputProps={{
          placeholder: filterFieldPlaceholder
        }}
      />
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
                {tableProps.getRowModel().rows.map((row) => {
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
                })}
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
            totalCount={tableOptions.data.length}
            isLoading={isLoading === undefined ? true : isLoading}
            goToPage={tableProps.setPageIndex}
            setPageSize={tableProps.setPageSize}
            sizePerPageList={SIZE_PER_PAGE_LIST}
          />
        )}
      </div>
    </div>
  );
}

export default observer(ClientTable);
