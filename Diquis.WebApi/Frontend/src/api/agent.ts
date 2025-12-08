import axios, { AxiosError, AxiosResponse } from "axios";
import { toast } from "react-toastify";
import createAuthRefreshInterceptor from "axios-auth-refresh";

import { store } from "stores/store";
import type { User, RegisterUserRequest, ChangePasswordRequest, UpdateProfileRequest, UpdatePreferencesRequest, ChangeProfileImageRequest, CurrentUser, SearchParams, PaginatedResult, Result, CreateTenantRequest, Tenant, AddProductRequest, Product, TokenData, UserLogin, ForgotPasswordRequest, ResetPasswordRequest } from "lib/types";
import sleep from "utils/sleep";
import i18n from '../i18n'; // Import i18n

// Base URL
// -- development: https://localhost:7250/api
// -- production: (your domain)
axios.defaults.baseURL = import.meta.env.VITE_API_URL;

// Send up the token with every request, when there is a token
axios.interceptors.request.use((config) => {
  const token = store.authStore.token;
  config.headers = {
    ...config.headers, // Preserve existing headers
    Tenant: store.authStore.tenant ?? "",
    'Accept-Language': i18n.language, // Add Accept-Language header
  };
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// Axios response interceptors
axios.interceptors.response.use(
  async (response) => {
    if (import.meta.env.MODE === "development") await sleep(1000); // Artificial delay for development
    return response;
  },
  async (error: AxiosError) => {
    if (!error.response) {
      return Promise.reject(error);
    }

    const { data, status } = error.response;

    switch (status) {
      case 400:
        toast.error("Error code 400: bad request");
        break;
      case 401:
        // 401 unauthorized is handled by axios-auth-refresh and refreshAuthLogic function
        break;
      case 500:
        toast.error("Error code 500: internal server error");
        console.info(data);
        break;
      default:
        break;
    }
    return Promise.reject(error);
  }
);

const refreshAuthLogic = async (failedRequest: any) => {
  try {
    const { refreshToken } = store.authStore;
    const response = await axios.get<Result<TokenData>>("/tokens/" + refreshToken);
    if (!response.data.succeeded) {
      store.accountStore.logout();
      return Promise.reject(failedRequest);
    }
    store.authStore.setToken(response.data.data.token);
    failedRequest.response.config.headers["Authorization"] = "Bearer " + response.data.data.token;
    return Promise.resolve();
  } catch (error) {
    store.accountStore.logout();
    return Promise.reject(error);
  }
};
createAuthRefreshInterceptor(axios, refreshAuthLogic);

const responseBody = <T>(response: AxiosResponse<T>) => response.data;

// Helper function to trigger file download from blob
const downloadBlob = (blob: any, filename: string) => {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = filename;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  window.URL.revokeObjectURL(url);
};

// Axios Base
const requests = {
  get: <T>(url: string) => axios.get<T>(url).then(responseBody),
  post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
  postBlob: <T>(url: string, body: {}) => axios.post<T>(url, body, { responseType: "blob" }),
  put: <T>(url: string, body: {}, options?: {}) => axios.put<T>(url, body, options).then(responseBody),
  del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
};

// Authentication & Profile Management (Current User)
const Account = {
  current: () => requests.get<Result<CurrentUser>>("/identity/profile"),
  login: (user: UserLogin) => requests.post<Result<TokenData>>(`/tokens`, user),
  refresh: (token: string) => requests.get<Result<TokenData>>(`/tokens/${token}`),
  update: (user: UpdateProfileRequest) => requests.put<Result<CurrentUser>>(`/identity/profile`, user),
  updateProfile: (user: UpdateProfileRequest) => requests.put<Result>(`/identity/profile`, user),
  changeProfileImage: (changeProfileImageRequest: ChangeProfileImageRequest) => {
    let formData = new FormData(); // form data to send up a file
    Object.entries(changeProfileImageRequest).forEach(([key, val]) => {
      formData.append(key, val);
    });
    return requests.put<Result<string>>("/identity/profile-image", formData, {
      headers: { "Content-type": "multipart/form-data" },
    });
  },
  updatePreferences: (updatePreferencesRequest: UpdatePreferencesRequest) => requests.put<Result>(`/identity/preferences`, updatePreferencesRequest),
  changePassword: (changePasswordRequest: ChangePasswordRequest) => requests.put<Result>(`/identity/change-password`, changePasswordRequest),
  forgotPassword: (forgotPasswordRequest: ForgotPasswordRequest) => requests.post<Result>(`/identity/forgot-password`, forgotPasswordRequest),
  resetPassword: (resetPasswordRequest: ResetPasswordRequest) => requests.post<Result>(`/identity/reset-password`, resetPasswordRequest),
};

// Products (sample business entity)
const Products = {
  search: (params: SearchParams) => requests.post<PaginatedResult<Product>>(`/products/products-paginated`, params), // paginated list handled server-side
  create: (product: AddProductRequest) => requests.post<Result<Product>>("/products", product),
  export: async (params?: any, filename = "products.xlsx") => {
    try {
      const response = await requests.postBlob("/products/export", params || {});
      downloadBlob(response.data, filename);
      return response;
    } catch (error) {
      console.error("Export failed:", error);
      throw error;
    }
  },
  details: (id: string) => requests.get<Result<Product>>(`/products/${id}`),
  update: (product: AddProductRequest, id: string) => requests.put<Result<Product>>(`/products/${id}`, product),
  delete: (id: string) => requests.del<Result<string>>(`/products/${id}`),
  generatePdf: (id: string) => requests.postBlob(`/products/pdf/${id}`, {}),

};

// User Management
const Users = {
  list: () => requests.get<Result<User[]>>("/identity/users"), // full list for client-side pagination
  create: (appUser: RegisterUserRequest) => requests.post<Result<User>>(`/identity/users`, appUser),
  details: (id: string) => requests.get<Result<User>>(`/identity/users/${id}`),
  update: (user: User) => requests.put<Result<User>>(`/identity/users/${user.id}`, user),
  delete: (id: string) => requests.del<Result<string>>(`/identity/users/${id}`),
};

// Tenant Management
const Tenants = {
  list: () => requests.get<Result<Tenant[]>>("/tenants"), // full list for client-side pagination
  listOptions: () => requests.get<Result<Tenant[]>>("/tenants/tenant-options"), // options for demo login

  details: (id: string) => requests.get<Result<Tenant>>(`/tenants/${id}`),
  create: (tenant: CreateTenantRequest) => requests.post<Result<Tenant>>(`/tenants`, tenant),
  update: (tenant: Tenant) => requests.put<Result<Tenant>>(`/tenants/${tenant.id}`, tenant),
};

const agent = {
  Account,
  Users,
  Tenants,
  Products,
};

export default agent;
