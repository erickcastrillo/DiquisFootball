import { Link } from 'react-router-dom';
import clsx from 'clsx';
import { observer } from 'mobx-react-lite';

import * as layoutConstants from 'constants';
import { useToggle, useViewPort } from 'hooks';
import { profileMenuItems } from './data';
import ProfileDropdown from './ProfileDropdown';
import logoSmall from 'assets/images/logo-sm.png';
import logo from 'assets/images/logo.png';
import { useStore } from 'stores/store';
import ThemeToggle from './ThemeToggle';
import LanguageSwitcher from 'components/LanguageSwitcher'; // Import LanguageSwitcher

type TopbarProps = {
  hideLogo?: boolean;
  navCssClasses?: string;
  openLeftMenuCallBack?: () => void;
  topbarDark?: boolean;
};

const Topbar = ({
  hideLogo,
  navCssClasses,
  openLeftMenuCallBack,
  topbarDark
}: TopbarProps) => {
  const { layoutStore, accountStore, authStore } = useStore();
  const { width } = useViewPort();
  const [, toggleMenu] = useToggle();

  const { layoutType, leftSideBarType } = layoutStore;
  const { currentUser } = accountStore;

  /**
   * Toggle the leftmenu when having mobile screen
   */
  const handleLeftMenuCallBack = () => {
    toggleMenu();
    if (openLeftMenuCallBack) openLeftMenuCallBack();
    switch (layoutType) {
      case layoutConstants.LayoutTypes.LAYOUT_VERTICAL:
        if (width >= 768) {
          if (leftSideBarType === 'fixed' || leftSideBarType === 'scrollable')
            layoutStore.changeSidebarType(
              layoutConstants.SideBarWidth.LEFT_SIDEBAR_TYPE_CONDENSED
            );
          if (leftSideBarType === 'condensed')
            layoutStore.changeSidebarType(
              layoutConstants.SideBarWidth.LEFT_SIDEBAR_TYPE_FIXED
            );
        }
        break;

      case layoutConstants.LayoutTypes.LAYOUT_FULL: {
        const htmlElement = document.querySelector('html');
        if (htmlElement) {
          htmlElement.classList.toggle('hide-menu');
        }
        break;
      }
      default:
        break;
    }
  };

  /**
   * Toggle full screen
   */
  const fullScreenListener = () => {
    document.body.classList.toggle('fullscreen-enable');
    if (!document.fullscreenElement) {
      if (document.documentElement.requestFullscreen) {
        document.documentElement.requestFullscreen();
      }
    } else {
      if (document['exitFullscreen']) {
        document['exitFullscreen']();
      }
    }
  };

  return (
    <div className={clsx('navbar-custom', navCssClasses)}>
      <div className={'topbar container-fluid'}>
        {/* Topbar Left Group */}
        <div className="d-flex align-items-center gap-lg-2 gap-1">
          <button
            className="button-toggle-menu"
            onClick={handleLeftMenuCallBack}
          >
            <i className="mdi mdi-menu" />
          </button>
          <ul className="breadcrumb m-0 d-none d-sm-flex">
            <li className="breadcrumb-item active">{authStore.title}</li>
          </ul>
        </div>
        {!hideLogo && (
          <Link to="/" className="topnav-logo">
            <span className="topnav-logo-lg">
              <img src={logo} alt="logo" height="16" />
            </span>
            <span className="topnav-logo-sm">
              <img
                src={logoSmall}
                alt="logo"
                height="16"
              />
            </span>
          </Link>
        )}

        <ul className="topbar-menu d-flex align-items-center gap-3">
          <li className="d-none d-sm-inline-block">
            <LanguageSwitcher /> {/* Add LanguageSwitcher component */}
          </li>
          <li className="d-none d-sm-inline-block">
            <ThemeToggle />
          </li>
          <li className="d-none d-md-inline-block">
            <button className="nav-link" onClick={fullScreenListener}>
              <i className="ri-fullscreen-line font-22"></i>
            </button>
          </li>

          <ProfileDropdown
            userImage={currentUser?.imageUrl || ''}
            menuItems={profileMenuItems}
            username={`${currentUser?.firstName} ${currentUser?.lastName}`}
          />
        </ul>
      </div>
    </div>
  );
};

export default observer(Topbar);
