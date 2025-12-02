import React, {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState
} from 'react';
import { useLocation } from 'react-router-dom';

import { findAllParent, findMenuItem } from 'utils';
import MenuItem from './MenuItem';
import type { MenuItemType } from 'layout/nav/types';
import { useStore } from 'stores/store';
import { Roles } from 'lib/types';
import MenuItemWithChildren from './MenuItemWithChildren';

type AppMenuProps = {
  menuItems: Array<MenuItemType>;
};

const AppMenu = ({ menuItems }: AppMenuProps) => {
  let location = useLocation();
  const { accountStore } = useStore();

  const menuRef = useRef<HTMLUListElement>(null);

  const [activeMenuItems, setActiveMenuItems] = useState<Array<string>>([]);

  const availableMenuOptions = useMemo(
    () =>
      menuItems.filter(
        (item) =>
          item.roles?.includes(
            accountStore.currentUser?.roleId || Roles.basic
          ) ||
          !item.roles ||
          item.roles?.length == 0
      ),
    [menuItems, accountStore.currentUser?.roleId]
  );

  /*
   * toggle the menus
   */
  const toggleMenu = (menuItem: MenuItemType, show: boolean) => {
    if (show)
      setActiveMenuItems([
        menuItem['key'],
        ...findAllParent(menuItems, menuItem)
      ]);
  };

  /**
   * get menu items
   */
  const activeMenu = useCallback(() => {
    const div = document.getElementById('main-side-menu');
    let matchingMenuItem = null;

    if (div) {
      let items: HTMLCollectionOf<HTMLAnchorElement> =
        div.getElementsByClassName(
          'side-nav-link-ref'
        ) as HTMLCollectionOf<HTMLAnchorElement>;
      for (let i = 0; i < items.length; ++i) {
        if (location.pathname === items[i].pathname) {
          matchingMenuItem = items[i];
          break;
        }
      }

      if (matchingMenuItem) {
        const mid = matchingMenuItem.getAttribute('data-menu-key');
        const activeMt = findMenuItem(menuItems, mid!);
        if (activeMt) {
          setActiveMenuItems([
            activeMt['key'],
            ...findAllParent(menuItems, activeMt)
          ]);
        }
      }
    }
  }, [location.pathname, menuItems]);

  useEffect(() => {
    activeMenu();
  }, [activeMenu]);

  return (
    <ul className="side-nav" ref={menuRef} id="main-side-menu">
      {availableMenuOptions.map((item, index) => {
        return (
          <React.Fragment key={index.toString()}>
            {item.isTitle ? (
              <li className="side-nav-title">{item.label}</li>
            ) : (
              <>
                {item.children ? (
                  <MenuItemWithChildren
                    item={item}
                    toggleMenu={toggleMenu}
                    subMenuClassNames="side-nav-second-level"
                    activeMenuItems={activeMenuItems}
                    linkClassName="side-nav-link"
                  />
                ) : (
                  <MenuItem
                    item={item}
                    linkClassName="side-nav-link"
                    className={`side-nav-item ${
                      activeMenuItems.includes(item.key)
                        ? 'menuitem-active'
                        : ''
                    }`}
                  />
                )}
              </>
            )}
          </React.Fragment>
        );
      })}
    </ul>
  );
};

export default AppMenu;
