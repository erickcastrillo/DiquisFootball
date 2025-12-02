import React from 'react';
import { ModalHeaderProps } from 'react-bootstrap';
import BsModal, {
  type ModalProps as BsModalProps
} from 'react-bootstrap/Modal';
import { ModalBodyProps } from 'react-bootstrap/esm/ModalBody';

type ModalProps = BsModalProps & {
  children: React.ReactNode;
  headerTitle: string;
  modalHeaderProps?: ModalHeaderProps;
  modalBodyProps?: ModalBodyProps;
};

const Modal = (props: ModalProps) => {
  const {
    modalHeaderProps = {},
    modalBodyProps = {},
    headerTitle,
    ...modalProps
  } = props;

  return (
    <BsModal
      show={props.show}
      onHide={props.onHide}
      size="lg"
      centered
      {...modalProps}
    >
      <BsModal.Header closeButton {...modalHeaderProps}>
        <h2 className="mb-2 fs-3">{headerTitle}</h2>
      </BsModal.Header>
      <BsModal.Body {...modalBodyProps}>{props.children}</BsModal.Body>
    </BsModal>
  );
};

export default Modal;
