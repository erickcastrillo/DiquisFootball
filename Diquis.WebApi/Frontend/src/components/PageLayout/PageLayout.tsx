import { useStore } from 'stores/store';
import React, { useEffect } from 'react';

type PageLayoutProps = {
  children: React.ReactNode;
  title: string;
  action?: JSX.Element;
};

const PageLayout = ({ title, children, action }: PageLayoutProps) => {
  const { authStore } = useStore();

  useEffect(() => {
    authStore.setTitle(title);
  }, []);

  return (
    <div className="d-flex flex-column">
      {!!action && (
        <div className="d-flex justify-content-end align-items-baseline mb-3">
          {action}
        </div>
      )}
      {children}
    </div>
  );
};

export default PageLayout;
