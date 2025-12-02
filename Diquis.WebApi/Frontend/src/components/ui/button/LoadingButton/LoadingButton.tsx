import React from 'react';
import { Button, ButtonProps, Spinner } from 'react-bootstrap';

type LoadingButtonProps = ButtonProps & {
  loading: boolean;
};

const LoadingButton = ({ loading, ...props }: LoadingButtonProps) => {
  return (
    <Button {...props}>
      <div className="d-flex align-items-center justify-content-center gap-2">
        {loading ? (
          <Spinner animation="border" role="status" size="sm">
            <span className="visually-hidden">Loading...</span>
          </Spinner>
        ) : (
          <></>
        )}
        <span>{props.children}</span>
      </div>
    </Button>
  );
};

export default LoadingButton;
