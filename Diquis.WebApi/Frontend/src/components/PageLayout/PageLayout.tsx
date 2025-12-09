import React from 'react';

type PageLayoutProps = {
  children: React.ReactNode;
  action?: JSX.Element;
  title?: string;
};

const PageLayout = ({ children, action, title }: PageLayoutProps) => {
  return (
    <div className="d-flex flex-column">
      {!!title && <h4 className="page-title mb-3">{title}</h4>}
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
