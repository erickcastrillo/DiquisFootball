import { Roles } from 'lib/types';
import { MenuItemType } from './types';

const sidebarMenuItems: MenuItemType[] = [
  {
    key: 'app-products',
    label: 'Products',
    icon: 'uil-box',
    url: '/products'
  },
  {
    key: 'app-user-management',
    label: 'User Management',
    icon: 'mdi mdi-account-outline',
    url: '/users',
    roles: [Roles.root, Roles.admin]
  },
  {
    key: 'app-tenants',
    label: 'Tenants',
    icon: 'ri-bubble-chart-line',
    url: '/tenants',
    roles: [Roles.root]
  },
  {
    key: 'template-resources',
    label: 'Template Resources',
    isTitle: true
  },
  {
    key: 'base-ui',
    label: 'Base UI',
    icon: 'uil-apps',
    children: [
      {
        key: 'base-ui-accordions',
        label: 'Accordions',
        url: '/ui/accordions',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-alerts',
        label: 'Alerts',
        url: '/ui/alerts',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-avatars',
        label: 'Avatars',
        url: '/ui/avatars',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-badges',
        label: 'Badges',
        url: '/ui/badges',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-buttons',
        label: 'Buttons',
        url: '/ui/buttons',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-cards',
        label: 'Cards',
        url: '/ui/cards',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-dropdowns',
        label: 'Dropdowns',
        url: '/ui/dropdowns',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-form-basic',
        label: 'Form Basic',
        url: '/ui/form-basic',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-form-advanced',
        label: 'Form Advanced',
        url: '/ui/form-advanced',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-list-group',
        label: 'List Group',
        url: '/ui/list-group',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-modals',
        label: 'Modals',
        url: '/ui/modals',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-offcanvas',
        label: 'Offcanvas',
        url: '/ui/offcanvas',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-pagination',
        label: 'Pagination',
        url: '/ui/pagination',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-popovers',
        label: 'Popovers',
        url: '/ui/popovers',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-progress',
        label: 'Progress',
        url: '/ui/progress',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-spinners',
        label: 'Spinners',
        url: '/ui/spinners',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-tables',
        label: 'Tables',
        url: '/ui/tables',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-tabs',
        label: 'Tabs',
        url: '/ui/tabs',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-tooltips',
        label: 'Tooltips',
        url: '/ui/tooltips',
        parentKey: 'base-ui'
      },
      {
        key: 'base-ui-typography',
        label: 'Typography',
        url: '/ui/typography',
        parentKey: 'base-ui'
      }
    ]
  },
  {
    key: 'extended-ui',
    label: 'Extended UI',
    icon: 'uil-minus-path',
    children: [
      {
        key: 'extended-ui-charts',
        label: 'Charts',
        url: '/ui/charts',
        parentKey: 'extended-ui'
      },
      {
        key: 'extended-ui-dashboard',
        label: 'Dashboard',
        url: '/ui/dashboard',
        parentKey: 'extended-ui'
      },
      {
        key: 'extended-ui-icons',
        label: 'Icons',
        url: '/ui/icons',
        parentKey: 'extended-ui',
        children: [
          {
            key: 'extended-ui-icons-remix-icons',
            label: 'Remix Icons',
            url: '/ui/icons/remix-icons',
            parentKey: 'extended-ui-icons'
          },
          {
            key: 'extended-ui-icons-mdi',
            label: 'Material Design',
            url: '/ui/icons/mdi',
            parentKey: 'extended-ui-icons'
          },
          {
            key: 'extended-ui-icons-unicon-icons',
            label: 'Unicons',
            url: '/ui/icons/unicons',
            parentKey: 'extended-ui-icons'
          }
        ]
      },
      {
        key: 'extended-ui-dragula',
        label: 'Dragula',
        url: '/ui/dragula',
        parentKey: 'extended-ui'
      },
      {
        key: 'extended-ui-range-sliders',
        label: 'Range Sliders',
        url: '/ui/range-sliders',
        parentKey: 'extended-ui'
      }
    ]
  }
];

export { sidebarMenuItems };
