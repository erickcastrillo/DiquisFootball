import clsx from 'clsx';

import { SubMenus } from './types';
import MenuItemLink from './MenuItemLink';

const MenuItem = ({ item, className, linkClassName }: SubMenus) => {
  return (
    <li className={clsx('side-nav-item', className)}>
      <MenuItemLink item={item} className={linkClassName} />
    </li>
  );
};

export default MenuItem;
