import { makeAutoObservable, runInAction } from 'mobx';

import agent from 'api/agent';
import type {
  ChangePasswordRequest,
  UpdatePreferencesRequest,
  UpdateProfileRequest,
  CurrentUser,
  ChangeProfileImageRequest,
  UserLogin,
  ForgotPasswordRequest,
  ResetPasswordRequest
} from 'lib/types';
import { store } from 'stores/store';
import router from 'router';
import { JWT, LayoutTheme, REFRESH_TOKEN } from 'constants';

// current user - edit profile, update preferences, change password, etc
export default class AccountStore {
  currentUser: CurrentUser | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  get isLoggedIn() {
    return !!this.currentUser; // shorthand cast to boolean
  }

  // login - get token, then set current user and push to products view
  login = async (creds: UserLogin) => {
    store.authStore.setTenant(creds.tenant);
    try {
      const response = await agent.Account.login(creds);
      if (!response.succeeded) throw new Error(response.messages[0]);
      store.authStore.setToken(response.data.token);
      store.authStore.setRefreshToken(response.data.refreshToken);
      const user = await agent.Account.current();
      runInAction(() => (this.currentUser = user.data));
      router.navigate('/products');
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  // set all local variables to blank, remove token, push user to login url
  logout = () => {
    store.authStore.setToken(null);
    store.usersStore.users = [];
    store.tenantsStore.tenants = [];
    window.localStorage.removeItem(JWT);
    window.localStorage.removeItem(REFRESH_TOKEN);
    this.currentUser = null;
    //store.layoutStore.changeThemeColor(LayoutTheme.LAYOUT_THEME_LIGHT);
    router.navigate('/');
  };

  getCurrentUser = async () => {
    try {
      const response = await agent.Account.current();
      if (!response.succeeded) throw new Error(response.messages[0]);
      runInAction(() => (this.currentUser = response.data));
      return this.currentUser;
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  updateProfileImage = async (
    changeProfileImageRequest: ChangeProfileImageRequest
  ) => {
    try {
      const response = await agent.Account.changeProfileImage(
        changeProfileImageRequest
      );
      if (!response.succeeded) throw new Error(response.messages[0]);
      runInAction(() => {
        if (this.currentUser) this.currentUser.imageUrl = response.data;
      });
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  updateCurrentUser = async (user: UpdateProfileRequest) => {
    try {
      const response = await agent.Account.updateProfile(user);
      if (!response.succeeded) throw new Error(response.messages[0]);
      runInAction(() => {
        this.currentUser = response.data;
      });
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  updatePreferences = async (
    updatePreferencesRequest: UpdatePreferencesRequest
  ) => {
    try {
      const response = await agent.Account.updatePreferences(
        updatePreferencesRequest
      );
      if (!response.succeeded) throw new Error(response.messages[0]);
      return response;
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  changePassword = async (changePasswordRequest: ChangePasswordRequest) => {
    try {
      const response = await agent.Account.changePassword(
        changePasswordRequest
      );
      if (!response.succeeded) throw new Error(response.messages[0]);
      return response;
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  forgotPassword = async (forgotPasswordRequest: ForgotPasswordRequest) => {
    store.authStore.setTenant(forgotPasswordRequest.tenant);
    try {
      const response = await agent.Account.forgotPassword(
        forgotPasswordRequest
      );
      if (!response.succeeded) throw new Error(response.messages[0]);
      return response;
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  resetPassword = async (resetPasswordRequest: ResetPasswordRequest) => {
    store.authStore.setTenant(resetPasswordRequest.tenant);
    try {
      const response = await agent.Account.resetPassword(resetPasswordRequest);
      if (!response.succeeded) throw new Error(response.messages[0]);
      return response;
    } catch (error) {
      console.error(error);
      throw error;
    }
  };
}
