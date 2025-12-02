import React from 'react';
import { observer } from 'mobx-react-lite';

import { LayoutTheme } from 'constants';
import { useStore } from 'stores/store';

const ThemeToggle = () => {
  const { layoutStore } = useStore();

  return (
    <div
      className="nav-link cursor-pointer"
      onClick={() => layoutStore.toggleThemeColor()}
    >
      {layoutStore.theme === LayoutTheme.LAYOUT_THEME_DARK ? (
        <i className="uil uil-sun font-22" />
      ) : (
        <i className="uil uil-moon font-22" />
      )}
    </div>
  );
};

export default observer(ThemeToggle);
