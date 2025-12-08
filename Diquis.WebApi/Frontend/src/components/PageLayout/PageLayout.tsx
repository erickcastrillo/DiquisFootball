import React from 'react';

type PageLayoutProps = {
  children: React.ReactNode;
  action?: JSX.Element;
};

const PageLayout = ({ children, action }: PageLayoutProps) => {
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
