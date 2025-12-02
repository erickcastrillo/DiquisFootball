import { useEffect } from 'react';
import NProgress from 'nprogress';
import { Spinner } from 'react-bootstrap';

interface IProps {
  content?: React.ReactNode;
}

const LoadingScreen = (props: IProps) => {
  NProgress.configure({
    showSpinner: true
  });

  useEffect(() => {
    NProgress.start();

    return () => {
      NProgress.done();
    };
  }, []);

  return (
    <div
      className="d-flex align-items-center justify-content-center"
      style={{ height: '100vh' }}
    >
      <div className="d-flex flex-column align-items-center justify-content-center h-100 gap-1">
        <Spinner />
        <h3>{props.content}</h3>
      </div>
    </div>
  );
};

export default LoadingScreen;
