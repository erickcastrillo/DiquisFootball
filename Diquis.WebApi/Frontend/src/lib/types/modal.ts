import { type ModalProps as BsModalProps } from 'react-bootstrap';

export type ModalProps = Required<Pick<BsModalProps, 'show' | 'onHide'>> &
  Omit<BsModalProps, 'show' | 'onHide'>;

export type EditModalProps<T> = ModalProps & {
  data: T;
};
