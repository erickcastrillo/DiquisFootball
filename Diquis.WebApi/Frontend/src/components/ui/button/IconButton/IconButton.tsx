import clsx from 'clsx';
import { Button, ButtonProps } from 'react-bootstrap';

type IconButtonProps = ButtonProps & {
  icon: string;
};

const IconButton = ({ icon, ...props }: IconButtonProps) => {
  return (
    <Button {...props}>
      <div className="d-flex align-items-center">
        <i className={clsx(icon, props.children && 'mr-2')} />
        <span>{props.children}</span>
      </div>
    </Button>
  );
};

export default IconButton;
