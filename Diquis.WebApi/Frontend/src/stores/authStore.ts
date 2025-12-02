import { JWT, REFRESH_TOKEN } from 'constants';
import { makeAutoObservable, reaction, runInAction } from 'mobx';

export default class CommonStore {
  token: string | null = window.localStorage.getItem(JWT); // also store refresh token
  refreshToken: string | null = window.localStorage.getItem(REFRESH_TOKEN);
  tenant: string | null = '';
  hasSubdomain: boolean = false; // set upon app loading
  title: string | null = '';
  appLoaded: boolean = false;

  constructor() {
    makeAutoObservable(this); // let MobX auto create interfaces

    reaction(
      // reaction only runs when there is a change to token. not on initialization
      () => this.token,
      (token) => {
        if (token) {
          window.localStorage.setItem(JWT, token);
        } else {
          window.localStorage.removeItem(JWT);
        }
      }
    );

    reaction(
      () => this.refreshToken,
      (refreshToken) => {
        if (refreshToken) {
          window.localStorage.setItem(REFRESH_TOKEN, refreshToken);
        } else {
          window.localStorage.removeItem(REFRESH_TOKEN);
        }
      }
    );
  }

  setToken = (token: string | null) => {
    runInAction(() => {
      this.token = token;
    });
  };

  setRefreshToken = (refreshToken: string | null) => {
    runInAction(() => {
      this.refreshToken = refreshToken;
    });
  };

  setTenant = (tenant: string | null) => {
    runInAction(() => {
      this.tenant = tenant;
    });
  };

  setTitle = (title: string | null) => {
    runInAction(() => {
      this.title = title;
    });
  };

  setAppLoaded = () => {
    runInAction(() => {
      this.appLoaded = true;
    });
  };

  setSubdomain = () => {
    runInAction(() => {
      this.hasSubdomain = true;
    });
  };
}
