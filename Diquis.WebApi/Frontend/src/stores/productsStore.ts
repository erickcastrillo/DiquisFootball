import { makeAutoObservable, runInAction } from "mobx";

import agent from "api/agent";
import type { AddProductRequest, Product, SearchParams, PaginatedResult } from "lib/types";

export default class ProductsStore {
  products: Product[] = [];
  productMetaData: Omit<PaginatedResult<Product>, "data"> | null = null;

  loading = false; // modal window buttons loading state
  loadingInitial = false; // list view table loading state
  loadingExport = false; // export loading state

  constructor() {
    makeAutoObservable(this);
  }

  // loading state setter
  setLoadingInitial = (state: boolean) => {
    runInAction(() => {
      this.loadingInitial = state;
    });
  };
  // loading state setter
  setLoading = (state: boolean) => {
    runInAction(() => {
      this.loading = state;
    });
  };
  // set pagination meta data
  setProductMetaData = (metaData: Omit<PaginatedResult<Product>, "data">) => {
    runInAction(() => {
      this.productMetaData = metaData;
    });
  };

  // export loading state setter
  setLoadingExport = (state: boolean) => {
    runInAction(() => {
      this.loadingExport = state;
    });
  };

  // load products - paginated list of products from api
  loadProducts = async ({ pageNumber = 1, pageSize = 5, keyword = "", sorting = [] }: Partial<SearchParams>) => {
    this.setLoadingInitial(true);
    try {
      const params: SearchParams = {
        pageNumber,
        pageSize,
        keyword,
        sorting,
      };
      const { data, ...metaData } = await agent.Products.search(params);
      runInAction(() => {
        this.products = data;
      });
      this.setProductMetaData(metaData);
      this.setLoadingInitial(false);
    } catch (error) {
      console.error(error);
      this.setLoadingInitial(false);
    }
  };

  createProduct = async (product: AddProductRequest) => {
    this.setLoading(true);
    try {
      const response = await agent.Products.create(product);
      if (!response.succeeded) throw new Error(response.messages[0]);
      this.setLoading(false);
    } catch (error) {
      console.error(error);
      this.setLoading(false);
      throw error;
    }
  };

  updateProduct = async (product: AddProductRequest, id: string) => {
    this.setLoading(true);
    try {
      const response = await agent.Products.update(product, id);
      if (!response.succeeded) throw new Error(response.messages[0]);
      this.setLoading(false);
    } catch (error) {
      console.error(error);
      this.setLoading(false);
      throw error;
    }
  };

  deleteProduct = async (id: string) => {
    this.setLoadingInitial(true);
    try {
      const response = await agent.Products.delete(id);
      if (!response.succeeded) throw new Error(response.messages[0]);
      this.setLoadingInitial(false);
    } catch (error) {
      console.error(error);
      this.setLoadingInitial(false);
      throw error;
    }
  };

  // export products to Excel
  exportProducts = async ({ keyword = "", sorting = [], filename = "products-export.xlsx" }: Partial<SearchParams & { filename?: string }> = {}) => {
    this.setLoadingExport(true);
    try {
      const exportParams = {
        keyword,
        sorting,
        // Note: We don't include pagination params for export as we want all filtered results
      };

      await agent.Products.export(exportParams, filename);
      this.setLoadingExport(false);
    } catch (error) {
      console.error("Export failed:", error);
      this.setLoadingExport(false);
      throw error;
    }
  };
  // export products to Excel
  generatePdf = async (id: string) => {
    this.setLoadingExport(true);
    try {
      const response = await agent.Products.generatePdf(id); // Assuming you have a pdf method in your agent

      const blob = response.data as Blob;

      const productId = id.slice(0, 6).toUpperCase();
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = `Product-${productId}.pdf`;
      link.click();
      window.URL.revokeObjectURL(url);

      // You'll need to import and use your toast library here
      // toast.success("PDF created successfully");

      this.setLoadingExport(false);
      return true;
    } catch (error) {
      this.setLoadingExport(false);
      // toast.error("PDF creation failed");
      console.error("PDF generation failed:", error);
      return false;
    }
  };
}
