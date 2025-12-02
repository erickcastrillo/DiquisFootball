import React, { Fragment, ReactNode } from 'react';

import type { Roles } from 'lib/types';
import { useStore } from 'stores/store';
import router from 'router';

interface AuthGuardProps {
  children: ReactNode;
  roles?: Roles[];
}

const AuthGuard = ({ children, roles }: AuthGuardProps) => {
  const {
    accountStore: { isLoggedIn, currentUser }
  } = useStore();

  if (!isLoggedIn) router.navigate('/login');
  if (roles && !roles.includes(currentUser?.roleId as Roles))
    router.navigate('/403');

  return <Fragment>{children}</Fragment>;
};

export default AuthGuard;
