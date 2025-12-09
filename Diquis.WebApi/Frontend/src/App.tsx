import * as React from 'react';

import { observer } from 'mobx-react-lite';
import { Outlet } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';

import LoadingScreen from 'components/LoadingScreen';
import ScrollToTop from 'utils/ScrollToTop';
import { useStore } from 'stores/store';
import { changeHtmlAttribute } from 'utils';
import LanguageSwitcher from 'components/LanguageSwitcher';
import { useTranslation } from 'react-i18next';
import { useSignalR } from 'hooks/useSignalR'; // Import SignalR hook

function App() {
  const { authStore, accountStore, layoutStore } = useStore();
  const isDevelopment: boolean =
    !import.meta.env.NODE_ENV || import.meta.env.NODE_ENV === 'development';

  const { theme, topBarTheme } = layoutStore;
  const { t } = useTranslation();

  // Initialize SignalR connection
  useSignalR();

  // get the current user (otherwise reloading browser will clear mobx)
  React.useEffect(() => {
    if (authStore.token) {
      // token present, get user from API

      if (accountStore.currentUser) return;
      accountStore.getCurrentUser().finally(() => authStore.setAppLoaded());
    } else {
      // no token present, not logged in
      authStore.setAppLoaded();
    }
  }, [authStore, accountStore]);

  // checking if there is a subdomain
  React.useEffect(() => {
    const host = window.location.host;
    const subdomain = host.split('.');
    const minimumSegments = isDevelopment ? 2 : 3; // adjust to your url structure needs

    if (subdomain.length == minimumSegments) {
      authStore.setSubdomain(); // set to true --> will hide the tenant selection on client login
    }
  }, []);

  // update theme / layout attributes in html element
  React.useEffect(() => {
    changeHtmlAttribute('data-bs-theme', theme);
  }, [theme]);

  React.useEffect(() => {
    changeHtmlAttribute('data-topbar-color', topBarTheme);
  }, [topBarTheme]);

  if (!authStore.appLoaded) return <LoadingScreen />;

  return (
    <>
      <ScrollToTop />
      <ToastContainer
        position="bottom-left"
        hideProgressBar
        draggable
        autoClose={3000}
        theme={layoutStore.theme}
      />
      <Outlet />
    </>
  );
}

export default observer(App);
