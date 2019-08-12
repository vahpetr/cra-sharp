import React from 'react';
import PropTypes from 'prop-types';
import Application from '../modules/Application';
import ModalLoading from '../components/ModalLoading';
import Link from '../components/Link';
import {
  Modal,
  ModalHeader,
  ModalContainer,
  ModalActions
} from '../components/Modal';

export default class RegistrationActivationPage extends React.Component {
  static propTypes = {
    history: PropTypes.shape({ push: PropTypes.func.isRequired }).isRequired,
    match: PropTypes.shape({
      params: PropTypes.shape({
        activationToken: PropTypes.string
      }).isRequired
    }).isRequired
  };

  state = {
    loading: true,
    error: ''
  };

  abortController = new AbortController();

  componentDidMount() {
    const { activationToken } = this.props.match.params;
    if (activationToken) {
      this.confirmation(activationToken);
    }
  }

  componentWillUnmount() {
    this.abortController.abort();
  }

  async confirmation(activationToken) {
    try {
      this.abortController.abort();
      this.setState({ loading: true });
      this.abortController = new AbortController();
      await Application.registrationConfirm(
        activationToken,
        this.abortController.signal
      );
      window.location.href = window.location.origin;
    } catch (error) {
      // eslint-disable-next-line no-console
      console.error(error);
      this.setState({ loading: false, error: error.message });
    }
  }

  render() {
    if (this.state.loading) {
      return <ModalLoading text="Выполняется подтверждение регистрации" />;
    }

    return (
      <ModalContainer>
        <Modal as="div">
          <ModalHeader>Ошибка при регистрации</ModalHeader>
          <p>
            Регистрационный токен некорректен, истёк срок или токен уже был использован.
          </p>
          <ModalActions>
            <Link to="/login">На страницу входа</Link>
          </ModalActions>
        </Modal>
      </ModalContainer>
    );
  }
}
