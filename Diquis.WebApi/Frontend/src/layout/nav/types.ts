import type { Roles } from 'lib/types';

export type ProfileOption = {
  label: string;
  icon: string;
  redirectTo?: string;
};

export type MenuItemType = {
  key: string;
  label: string;
  icon?: string;
  url?: string;
  isTitle?: boolean;
  badge?: {
    variant: string;
    text: string;
  };
  parentKey?: string;
  target?: string;
  children?: MenuItemType[];
  roles?: Roles[];
};
