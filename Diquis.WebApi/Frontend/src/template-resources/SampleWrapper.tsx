import React from 'react';

type SampleWrapperProps = {
  title: string;
  children: React.ReactNode;
  subText?: React.ReactNode;
};

const SampleWrapper = (props: SampleWrapperProps) => {
  return (
    <div className="card">
      <div className="card-body">
        <h4 className="header-title">{props.title}</h4>
        {!!props.subText && (
          <p className="text-muted font-14 mb-3">{props.subText}</p>
        )}
        <div>{props.children}</div>
      </div>
    </div>
  );
};

export default SampleWrapper;
