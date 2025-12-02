import React, { Fragment, ReactNode } from 'react';
import { useStore } from '../../stores/store';
import router from 'router';

interface GuestGuardProps {
  children: ReactNode;
}

// this handles the initial redirection
const GuestGuard = ({ children }: GuestGuardProps) => {
  const {
    accountStore: { isLoggedIn }
  } = useStore();

  if (isLoggedIn) router.navigate('/products');

  return <Fragment>{children}</Fragment>;
};

export default GuestGuard;
