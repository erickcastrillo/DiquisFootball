import { makeAutoObservable, runInAction } from "mobx";

import agent from "api/agent";
import type { CreateTenantRequest, Tenant } from "lib/types";

export default class TenantsStore {
  tenants: Tenant[] = [];
  tenantOptions: Tenant[] = []; // only active tenants for login dropdown

  loading = false;
  loadingInitial = false;

  constructor() {
    makeAutoObservable(this);
  }

  // loading setter (initial page load)
  setLoadingInitial = (state: boolean) => {
    runInAction(() => {
      this.loadingInitial = state;
    });
  };

  // loading setter
  setLoading = (state: boolean) => {
    runInAction(() => {
      this.loading = state;
    });
  };

  // return an array sorted by created on date
  get tenantsSorted() {
    return Array.from(this.tenants.values()).sort((a, b) => new Date(b.createdOn).valueOf() - new Date(a.createdOn).valueOf());
  }

  // load tenants for admin vuew
  loadTenants = async () => {
    this.setLoadingInitial(true);
    try {
      const response = await agent.Tenants.list();
      if (!response.succeeded) throw new Error(response.messages[0]);
      runInAction(() => {
        this.tenants = response.data;
      });
      this.setLoadingInitial(false);
    } catch (error) {
      console.error(error);
      this.setLoadingInitial(false);
      throw error;
    }
  };

  // load tenant options for login dropdown
  loadTenantOptions = async () => {
    this.setLoadingInitial(true);
    try {
      const response = await agent.Tenants.listOptions();
      if (!response.succeeded) throw new Error(response.messages[0]);
      runInAction(() => {
        this.tenantOptions = response.data;
      });
      this.setLoadingInitial(false);
    } catch (error) {
      console.error(error);
      this.setLoadingInitial(false);
      throw error;
    }
  };

  // create new tenant
  createTenant = async (createTenantRequest: CreateTenantRequest) => {
    this.setLoading(true);

    try {
      const response = await agent.Tenants.create(createTenantRequest);
      if (!response.succeeded) throw new Error(response.messages[0]);
      const newTenant = response.data;
      runInAction(() => {
        this.tenants.push(newTenant); // add to registry list (local memory) - prevents having to reload the table
      });
      this.setLoading(false);
    } catch (error) {
      console.error(error);
      this.setLoading(false);
      throw error;
    }
  };

  // update tenant
  updateTenant = async (tenant: Tenant) => {
    this.setLoading(true);
    try {
      const response = await agent.Tenants.update(tenant);
      if (!response.succeeded) throw new Error(response.messages[0]);
      runInAction(() => {
        const tenantIndex = this.tenants.findIndex((x) => x.id == tenant.id);
        this.tenants[tenantIndex] = tenant;
      });
      this.setLoading(false);
    } catch (error) {
      console.error(error);
      this.setLoading(false);
      throw error;
    }
  };
}
