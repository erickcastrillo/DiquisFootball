import { ProfileOption } from 'layout/nav/types';

// get the profilemenu
const profileMenuItems: ProfileOption[] = [
  {
    label: 'Profile',
    icon: 'mdi mdi-account-circle',
    redirectTo: '/profile?tab=user-info'
  },
  {
    label: 'Preferences',
    icon: 'mdi mdi-account-edit',
    redirectTo: '/profile?tab=preferences'
  },
  {
    label: 'Password',
    icon: 'mdi mdi-lifebuoy',
    redirectTo: '/profile?tab=password'
  },
  {
    label: 'Logout',
    icon: 'mdi mdi-logout',
    redirectTo: '/account/logout'
  }
];

export { profileMenuItems };
