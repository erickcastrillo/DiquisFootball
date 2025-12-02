import { createContext, useContext } from 'react';
import UsersStore from './usersStore';
import AuthStore from './authStore';
import TenantsStore from './tenantsStore';
import AccountUserStore from './accountStore';
import ProductsStore from './productsStore';
import LayoutStore from './layoutStore';

interface Store {
  authStore: AuthStore;
  accountStore: AccountUserStore;
  usersStore: UsersStore;
  tenantsStore: TenantsStore;
  productsStore: ProductsStore;
  layoutStore: LayoutStore;
}

// Base Mobx Store
export const store: Store = {
  authStore: new AuthStore(),
  accountStore: new AccountUserStore(),
  usersStore: new UsersStore(),
  tenantsStore: new TenantsStore(),
  productsStore: new ProductsStore(),
  layoutStore: new LayoutStore()
};

export const StoreContext = createContext(store);

// Hook to access Mobx store
export function useStore() {
  return useContext(StoreContext);
}
