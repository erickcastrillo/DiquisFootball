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

  // load tenants for admin view
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

  // create new tenant (now returns tenant ID, provisioning happens in background)
  createTenant = async (createTenantRequest: CreateTenantRequest) => {
    this.setLoading(true);

    try {
      const response = await agent.Tenants.create(createTenantRequest);
      if (!response.succeeded) throw new Error(response.messages[0]);
      
      // Response now contains tenant ID (string), not full Tenant object
      // The tenant will be added to the list when we receive SignalR notification
      // or when we manually refresh the list
      await this.loadTenants(); // Refresh to get the tenant with Pending status
      
      this.setLoading(false);
    } catch (error) {
      console.error(error);
      this.setLoading(false);
      throw error;
    }
  };

  // update tenant (now happens in background)
  updateTenant = async (tenant: Tenant) => {
    this.setLoading(true);
    try {
      const response = await agent.Tenants.update(tenant);
      if (!response.succeeded) throw new Error(response.messages[0]);
      
      // Response now contains tenant ID (string), not full Tenant object
      // The tenant will be updated when we receive SignalR notification
      await this.loadTenants(); // Refresh to get the tenant with Updating status
      
      this.setLoading(false);
    } catch (error) {
      console.error(error);
      this.setLoading(false);
      throw error;
    }
  };
}
