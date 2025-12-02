import React, { useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import SimpleBar from 'simplebar-react';

import AppMenu from './Menu';
import logo from 'assets/images/logo.png';
import logoSm from 'assets/images/logo-sm.png';
import profileImg from 'assets/images/users/avatar-1.jpg';
import { sidebarMenuItems } from './menu-options';

type SideBarContentProps = {
  hideUserProfile: boolean;
};

const SideBarContent = ({ hideUserProfile }: SideBarContentProps) => {
  return (
    <>
      {!hideUserProfile && (
        <div className="leftbar-user">
          <Link to="/">
            <img
              src={profileImg}
              alt=""
              height="42"
              className="rounded-circle shadow-sm"
            />
            <span className="leftbar-user-name">Dominic Keller</span>
          </Link>
        </div>
      )}
      <AppMenu menuItems={sidebarMenuItems} />
    </>
  );
};

type SidebarNavProps = {
  hideLogo?: boolean;
  hideUserProfile: boolean;
  isCondensed: boolean;
};

const SidebarNav = ({
  isCondensed,
  hideLogo,
  hideUserProfile
}: SidebarNavProps) => {
  const menuNodeRef = useRef<HTMLDivElement>(null);

  /**
   * Handle the click anywhere in doc
   */
  const handleOtherClick = (e: MouseEvent) => {
    if (
      menuNodeRef &&
      menuNodeRef.current &&
      menuNodeRef.current.contains(e.target as Node)
    )
      return;
    // else hide the menubar
    const htmlElement = document.querySelector('html');
    if (htmlElement) {
      htmlElement.classList.remove('sidebar-enable');
    }
  };

  useEffect(() => {
    document.addEventListener('mousedown', handleOtherClick, false);

    return () => {
      document.removeEventListener('mousedown', handleOtherClick, false);
    };
  }, []);

  return (
    <div className="leftside-menu" ref={menuNodeRef}>
      {!hideLogo && (
        <>
          <Link to="/" className="logo text-center">
            <span className="logo-lg">
              <img src={logo} alt="logo" style={{ height: 30 }} />
            </span>
            <span className="logo-sm">
              <img src={logoSm} alt="logo" style={{ height: 30 }} />
            </span>
          </Link>
        </>
      )}

      {!isCondensed && (
        <SimpleBar
          style={{ maxHeight: '100%' }}
          timeout={500}
          scrollbarMaxSize={320}
        >
          <SideBarContent hideUserProfile={hideUserProfile} />
        </SimpleBar>
      )}
      {isCondensed && <SideBarContent hideUserProfile={hideUserProfile} />}
    </div>
  );
};

export default SidebarNav;
