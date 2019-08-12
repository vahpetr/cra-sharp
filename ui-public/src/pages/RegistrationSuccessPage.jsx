import React from 'react';
import Link from '../components/Link';
import {
  Modal,
  ModalHeader,
  ModalActions,
  ModalContainer
} from '../components/Modal';

export default function RegistrationSuccessPage() {
  return (
    <ModalContainer>
      <Modal as="div">
        <ModalHeader>Запрос регистрации отправлен</ModalHeader>
        <p>Вам было отправлено письмо.</p>
        <p>
          Чтобы завершить регистрацию, проверьте свой почтовый ящик и следуйте
          инструкциям в письме.
        </p>
        <p>
          Если письмо не пришло, проверьте папку с <b>нежелательной почтой</b>.
          <br />
          Возможно письмо попало в спам.
        </p>
        <ModalActions>
          <Link to="/login">На страницу входа</Link>
        </ModalActions>
      </Modal>
    </ModalContainer>
  );
}
