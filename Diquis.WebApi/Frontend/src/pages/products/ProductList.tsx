import { useCallback, useEffect, useMemo, useState } from "react";
import { observer } from "mobx-react-lite";
import { Card, Button, Tooltip, TooltipProps, OverlayTrigger } from "react-bootstrap";
import { PaginationState, SortingState, getCoreRowModel } from "@tanstack/react-table";
import { useTranslation } from "react-i18next";

import { useStore } from "stores/store";
import { Roles } from "lib/types";
import getProductColumnShape from "./ProductColumnShape";
import ProductModal from "./ProductModal";
import ServerTable from "components/tables/ServerTable/ServerTable";
import PageLayout from "components/PageLayout";
import TableFilters from "components/tables/ServerTable/TableFilters";
import { useModal } from "hooks";

const ProductList = () => {
  const { productsStore, accountStore, layoutStore, authStore } = useStore();
  const { loadProducts, products, productMetaData, loadingInitial, exportProducts, loadingExport } = productsStore;
  const { t, i18n } = useTranslation();

  useEffect(() => {
    authStore.setTitle(t('products.title'));
  }, [t, authStore, i18n.language]);

  const { show, onShow, onHide } = useModal();
  const [searchQuery, setSearchQuery] = useState("");
  const [filteredQuery, setFilteredQuery] = useState("");

  const [pagination, setPagination] = useState<PaginationState>({
    pageIndex: 0,
    pageSize: layoutStore.pageSize,
  });
  const [sorting, setSorting] = useState<SortingState>([]);

  const data = useMemo(() => products || [], [products]);
  const columns = useMemo(() => getProductColumnShape({ pagination, filteredQuery, sorting, t }), [pagination, filteredQuery, sorting, t]);

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
      {t('products.basicUserTooltip')}
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

      console.log("Export completed successfully");
    } catch (error) {
      console.error("Export failed:", error);
    }
  }, [exportProducts, filteredQuery, sorting]);

  return (
    <PageLayout
      action={
        accountStore.currentUser?.roleId !== Roles.basic ? (
          <Button variant="primary" onClick={onShow}>
            {t('products.newProduct')}
          </Button>
        ) : (
          <OverlayTrigger placement="bottom-start" delay={{ show: 250, hide: 400 }} overlay={renderTooltip}>
            <span>
              <Button disabled variant="primary">
                {t('products.newProduct')}
              </Button>
            </span>
          </OverlayTrigger>
        )
      }
    >
      <Card className="pt-2">
        <Card.Body>
          <TableFilters searchInputPlaceholder={t('products.searchPlaceholder')} handleClearFilters={handleClearFilters} searchQuery={searchQuery} setSearchQuery={setSearchQuery} filteredQuery={filteredQuery} setFilteredQuery={setFilteredQuery} handleFilter={handleFilter} />

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
        <small>{t('products.paginationDescription')}</small>
        <Button variant="success" onClick={handleExport} disabled={loadingExport}>
          {loadingExport ? t('products.exporting') : t('products.export')}
        </Button>
      </div>
    </PageLayout>
  );
};

export default observer(ProductList);
