import React from 'react';
import Loading from './Loading';
import { Modal, ModalHeader, ModalContainer } from './Modal';

export default function ModalLoading(props) {
  return (
    <ModalContainer>
      <Modal>
        <ModalHeader>
          <Loading {...props} />
        </ModalHeader>
      </Modal>
    </ModalContainer>
  );
}
