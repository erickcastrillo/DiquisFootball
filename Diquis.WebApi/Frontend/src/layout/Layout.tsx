import React, { Suspense, useCallback, useEffect } from "react";
import { Container } from "react-bootstrap";
import { observer } from "mobx-react-lite";

import { useToggle, useViewPort } from "hooks";
import { changeHtmlAttribute } from "utils/dom";
import * as layoutConstants from "constants/layout";
import { useStore } from "stores/store";

const Topbar = React.lazy(() => import("./nav/Topbar"));
const Footer = React.lazy(() => import("./Footer"));
const SidebarNav = React.lazy(() => import("./nav/SidebarNav"));

const loading = () => <div className=""></div>;

type LayoutProps = {
  children: React.ReactNode;
};

const Layout = ({ children }: LayoutProps) => {
  const { width } = useViewPort();
  const [isMenuOpened, toggleMenu] = useToggle();
  const { layoutStore } = useStore();

  const { theme, leftSideBarTheme, leftSideBarType, layoutWidth, topBarTheme } = layoutStore;

  /*
   * layout defaults
   */
  useEffect(() => {
    changeHtmlAttribute("data-layout", layoutConstants.LayoutTypes.LAYOUT_VERTICAL);
  }, []);

  useEffect(() => {
    changeHtmlAttribute("data-bs-theme", theme);
    changeHtmlAttribute("data-topbar-color", topBarTheme);
  }, [theme, topBarTheme]);

  useEffect(() => {
    changeHtmlAttribute("data-layout-mode", layoutWidth);
  }, [layoutWidth]);

  useEffect(() => {
    changeHtmlAttribute("data-menu-color", leftSideBarTheme);
    changeHtmlAttribute("data-topbar-color", topBarTheme);
  }, [leftSideBarTheme, topBarTheme]);

  useEffect(() => {
    changeHtmlAttribute("data-sidenav-size", leftSideBarType);
    changeHtmlAttribute("data-sidenav-static-size", leftSideBarType);
  }, [leftSideBarType]);

  /**
   * Open the menu when having mobile screen
   */
  const openMenu = () => {
    toggleMenu();
    const htmlElement = document.querySelector("html");
    if (htmlElement) {
      if (isMenuOpened) {
        htmlElement.classList.remove("sidebar-enable");
      } else {
        htmlElement.classList.add("sidebar-enable");
      }
    }
  };

  const updateDimensions = useCallback(() => {
    // activate the condensed sidebar if smaller devices like ipad or tablet
    if (width <= 768) {
      layoutStore.changeSidebarType(layoutConstants.SideBarWidth.LEFT_SIDEBAR_TYPE_FULL);
    } else if (width > 768 && width <= 1028) {
      layoutStore.changeSidebarType(layoutConstants.SideBarWidth.LEFT_SIDEBAR_TYPE_CONDENSED);
    } else if (width > 1028) {
      layoutStore.changeSidebarType(layoutConstants.SideBarWidth.LEFT_SIDEBAR_TYPE_FIXED);
    }
  }, [width]);

  useEffect(() => {
    window.addEventListener("resize", updateDimensions);
    updateDimensions();
    return () => {
      window.removeEventListener("resize", updateDimensions);
    };
  }, [updateDimensions]);

  const isCondensed = leftSideBarType === layoutConstants.SideBarWidth.LEFT_SIDEBAR_TYPE_CONDENSED;

  return (
    <>
      <div className="wrapper">
        <Suspense fallback={loading()}>
          <Topbar openLeftMenuCallBack={openMenu} hideLogo />
        </Suspense>
        <Suspense fallback={loading()}>
          <SidebarNav isCondensed={isCondensed} hideUserProfile={true} />
        </Suspense>
        <div className="content-page">
          <div className="content pt-3">
            <Container fluid>{children}</Container>
            <Suspense fallback={loading()}>
              <Footer />
            </Suspense>
          </div>
        </div>
      </div>
    </>
  );
};
export default observer(Layout);
