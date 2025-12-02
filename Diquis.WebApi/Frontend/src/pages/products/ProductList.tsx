import { useCallback, useEffect, useMemo, useState } from "react";
import { observer } from "mobx-react-lite";
import { Card, Button, Tooltip, TooltipProps, OverlayTrigger } from "react-bootstrap";
import { PaginationState, SortingState, getCoreRowModel } from "@tanstack/react-table";

import { useStore } from "stores/store";
import { Roles } from "lib/types";
import getProductColumnShape from "./ProductColumnShape";
import ProductModal from "./ProductModal";
import ServerTable from "components/tables/ServerTable/ServerTable";
import PageLayout from "components/PageLayout";
import TableFilters from "components/tables/ServerTable/TableFilters";
import { useModal } from "hooks";

const ProductList = () => {
  const { productsStore, accountStore, layoutStore } = useStore();
  const { loadProducts, products, productMetaData, loadingInitial, exportProducts, loadingExport } = productsStore;

  const { show, onShow, onHide } = useModal();
  const [searchQuery, setSearchQuery] = useState("");
  const [filteredQuery, setFilteredQuery] = useState("");

  const [pagination, setPagination] = useState<PaginationState>({
    pageIndex: 0,
    pageSize: layoutStore.pageSize,
  });
  const [sorting, setSorting] = useState<SortingState>([]);

  const data = useMemo(() => products, [products]);
  const columns = useMemo(() => getProductColumnShape({ pagination, filteredQuery, sorting }), [getProductColumnShape, pagination, filteredQuery, sorting]);

  const handleFilter = useCallback(() => {
    setSearchQuery("");
    setFilteredQuery(searchQuery);
    loadProducts({
      pageNumber: pagination.pageIndex + 1,
      pageSize: pagination.pageSize,
      keyword: searchQuery,
    });
  }, [pagination, searchQuery]);

  const handleClearFilters = useCallback(() => {
    setFilteredQuery("");
    loadProducts({
      pageNumber: pagination.pageIndex + 1,
      pageSize: pagination.pageSize,
    });
  }, [pagination]);

  const renderTooltip = (props: TooltipProps) => (
    <Tooltip id="button-tooltip" className="font-14" {...props}>
      Basic user cannot use this feature
    </Tooltip>
  );

  useEffect(() => {
    loadProducts({
      pageNumber: pagination.pageIndex + 1,
      pageSize: pagination.pageSize,
      keyword: filteredQuery,
      sorting,
    });
  }, [loadProducts, pagination.pageSize, pagination.pageIndex, filteredQuery, sorting]);

  const handleExport = useCallback(async () => {
    try {
      await exportProducts({
        keyword: filteredQuery,
        sorting: sorting,
        filename: "products-export.xlsx",
      });

      // Optional: Show success message
      console.log("Export completed successfully");
    } catch (error) {
      console.error("Export failed:", error);
      // Optional: Show error toast/notification
    }
  }, [exportProducts, filteredQuery, sorting]);

  return (
    <PageLayout
      title="Products"
      action={
        accountStore.currentUser?.roleId !== Roles.basic ? (
          <Button variant="primary" onClick={onShow}>
            New Product
          </Button>
        ) : (
          <OverlayTrigger placement="bottom-start" delay={{ show: 250, hide: 400 }} overlay={renderTooltip}>
            <span>
              <Button disabled variant="primary">
                New Product
              </Button>
            </span>
          </OverlayTrigger>
        )
      }
    >
      <Card className="pt-2">
        <Card.Body>
          {/* Page Header */}
          <TableFilters searchInputPlaceholder="Search products..." handleClearFilters={handleClearFilters} searchQuery={searchQuery} setSearchQuery={setSearchQuery} filteredQuery={filteredQuery} setFilteredQuery={setFilteredQuery} handleFilter={handleFilter} />

          {show && (
            <ProductModal
              show={show}
              onHide={onHide}
              isEdit={false}
              paginationState={{
                pageIndex: pagination.pageIndex + 1,
                pageSize: pagination.pageSize,
              }}
              filteredQuery={filteredQuery}
              sorting={sorting}
            />
          )}

          <ServerTable data={data} columns={columns} getCoreRowModel={getCoreRowModel()} sorting={sorting} setSorting={setSorting} pagination={pagination} setPagination={setPagination} pageCount={productMetaData?.totalPages ?? 0} totalCount={productMetaData?.totalCount ?? 0} isLoading={loadingInitial} />
        </Card.Body>
      </Card>
      <div className="d-flex justify-content-between align-items-center mt-2 mb-4 w-full">
        <small>Server-side pagination with sorting & filtering</small>
        <Button variant="success" onClick={handleExport} disabled={loadingExport}>
          {loadingExport ? "Exporting..." : "Export"}
        </Button>
      </div>
    </PageLayout>
  );
};

export default observer(ProductList);
