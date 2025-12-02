import { makeAutoObservable, reaction, runInAction } from 'mobx';
import {
  LayoutTheme,
  LayoutTypes,
  LayoutWidth,
  TopBarTheme,
  SideBarTheme,
  SideBarWidth
} from 'constants';
import { PAGE_SIZE_KEY, THEME_KEY } from 'constants';

export default class LayoutStore {
  layoutType = LayoutTypes.LAYOUT_VERTICAL;
  layoutWidth = LayoutWidth.LAYOUT_WIDTH_FLUID;
  topBarTheme = TopBarTheme.TOPBAR_THEME_LIGHT;
  leftSideBarTheme = SideBarTheme.LEFT_SIDEBAR_THEME_DARK;
  leftSideBarType = SideBarWidth.LEFT_SIDEBAR_TYPE_FIXED;
  showRightSidebar = false;
  pageSize = 10;
  theme = LayoutTheme.LAYOUT_THEME_LIGHT;

  // Let Mobx auto create the interface for this class
  constructor() {
    makeAutoObservable(this);
    reaction(
      // this reaction only runs when there is a change to theme. not on initialization
      () => this.theme,
      (theme) => {
        window.localStorage.setItem(THEME_KEY, theme);
      }
    );

    const pageSizeStorage = window.localStorage.getItem(PAGE_SIZE_KEY);
    const themeStorage = window.localStorage.getItem(THEME_KEY);
    if (pageSizeStorage) {
      this.pageSize = parseInt(pageSizeStorage);
    }
    if (themeStorage) {
      this.theme = themeStorage as LayoutTheme;
    }
  }

  changeSidebarType = (state: SideBarWidth) => {
    runInAction(() => {
      this.leftSideBarType = state;
    });
  };

  changeThemeColor = (state: LayoutTheme) => {
    runInAction(() => {
      this.theme = state;
    });
  };

  toggleThemeColor = () => {
    runInAction(() => {
      this.theme =
        this.theme === LayoutTheme.LAYOUT_THEME_DARK
          ? LayoutTheme.LAYOUT_THEME_LIGHT
          : LayoutTheme.LAYOUT_THEME_DARK;
    });
  };

  changePageSize = (state: number) => {
    runInAction(() => {
      this.pageSize = state;
      window.localStorage.setItem(PAGE_SIZE_KEY, state.toString());
    });
  };

  changeTheme = (state: LayoutTheme) => {
    runInAction(() => {
      this.theme = state;
      window.localStorage.setItem(THEME_KEY, state.toString());
    });
  };
}
