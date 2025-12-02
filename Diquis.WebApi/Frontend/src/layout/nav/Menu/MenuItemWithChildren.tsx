import React, { useCallback, useEffect, useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import classNames from 'classnames';
import { Collapse } from 'react-bootstrap';
import { useStore } from 'stores/store';
import { SideBarWidth } from 'constants';
import { MenuItemType } from '../types';
import MenuItem from './MenuItem';

export type SubMenus = {
  item: MenuItemType;
  linkClassName?: string;
  subMenuClassNames?: string;
  activeMenuItems?: Array<string>;
  toggleMenu?: (item: MenuItemType, status: boolean) => void;
  className?: string;
};

const MenuItemWithChildren = ({
  item,
  linkClassName,
  subMenuClassNames,
  activeMenuItems,
  toggleMenu
}: SubMenus) => {
  const [open, setOpen] = useState<boolean>(
    activeMenuItems!.includes(item.key)
  );

  const { layoutStore } = useStore();

  const collapseClass =
    layoutStore.leftSideBarType === SideBarWidth.LEFT_SIDEBAR_TYPE_CONDENSED;

  useEffect(() => {
    setOpen(activeMenuItems!.includes(item.key));
  }, [activeMenuItems, item]);

  const toggleMenuItem = (
    e: React.MouseEvent<HTMLAnchorElement, MouseEvent>
  ) => {
    e.preventDefault();
    const status = !open;
    setOpen(status);
    if (toggleMenu) toggleMenu(item, status);
    return false;
  };

  return (
    <li className={classNames('side-nav-item', { 'menuitem-active': open })}>
      <Link
        to=""
        onClick={toggleMenuItem}
        data-menu-key={item.key}
        aria-expanded={open}
        className={linkClassName}
      >
        {item.icon && <i className={item.icon}></i>}
        {!item.badge ? (
          <span className="menu-arrow"></span>
        ) : (
          <span className={`badge bg-${item.badge.variant} float-end`}>
            {item.badge.text}
          </span>
        )}
        <span> {item.label} </span>
      </Link>
      <Collapse in={open}>
        <div className={collapseClass ? 'collapse' : ''}>
          <ul className={classNames(subMenuClassNames)} id="sidebarDashboards">
            {(item.children || []).map((child, index) => {
              return (
                <React.Fragment key={index.toString()}>
                  {child.children ? (
                    <MenuItemWithChildren
                      item={child}
                      linkClassName={
                        activeMenuItems!.includes(child.key) ? 'active' : ''
                      }
                      activeMenuItems={activeMenuItems}
                      subMenuClassNames="side-nav-third-level"
                      toggleMenu={toggleMenu}
                    />
                  ) : (
                    <MenuItem
                      item={child}
                      className={
                        activeMenuItems!.includes(child.key)
                          ? 'menuitem-active'
                          : ''
                      }
                      linkClassName={
                        activeMenuItems!.includes(child.key) ? 'active' : ''
                      }
                    />
                  )}
                </React.Fragment>
              );
            })}
          </ul>
        </div>
      </Collapse>
    </li>
  );
};

export default MenuItemWithChildren;
