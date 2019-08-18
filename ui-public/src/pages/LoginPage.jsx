import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components/macro';
import Application from '../modules/Application';
import Link from '../components/Link';
import Input from '../components/Input';
import CheckBox from '../components/CheckBox';
import Button from '../components/Button';
import ModalLoading from '../components/ModalLoading';
import LoginIcon from '../icons/LoginIcon';
import {
  Modal,
  ModalActions,
  ModalField,
  ModalHeader,
  ModalContainer,
  ModalError,
  ModalFieldError
} from '../components/Modal';

export default class LoginPage extends React.Component {
  static propTypes = {
    history: PropTypes.shape({ push: PropTypes.func.isRequired }).isRequired
  };

  formRef = React.createRef();
  // focus = false;
  state = {
    loading: false,
    error: '',
    emailError: '',
    passwordError: '',
    validated: false,
    isValid: false
  };

  abortController = new AbortController();

  componentWillUnmount() {
    this.abortController.abort();
  }

  onEmailChange = () => {
    if (this.state.validated) {
      const { email } = this.formRef.current;
      this.setState({
        emailError: email.validationMessage,
        isValid: this.formRef.current.checkValidity()
      });
    }
  };

  onPasswordChange = () => {
    if (this.state.validated) {
      const { password } = this.formRef.current;
      this.setState({
        passwordError: password.validationMessage,
        isValid: this.formRef.current.checkValidity()
      });
    }
  };

  // onFocus = () => {
  //   this.focus = true;
  // };

  // onBlur = () => {
  //   this.focus = false;
  // };

  // onInvalid = e => {
  //   if (!this.focus) {
  //     e.target.focus();
  //   }
  // };

  onSubmit = e => {
    e.preventDefault();
    const { email, password, remember } = e.target;

    const isValid = this.formRef.current.checkValidity();
    this.setState({
      emailError: email.validationMessage,
      passwordError: password.validationMessage,
      validated: true,
      isValid
    });

    if (isValid) {
      this.login(email.value, password.value, remember.checked);
    }
  };

  canSubmit = () => {
    if (this.state.loading) {
      return false;
    }
    const { validated, isValid } = this.state;
    return !validated || isValid;
  };

  async login(email, password, rememberMe) {
    try {
      this.abortController.abort();
      this.setState({ loading: true });
      this.abortController = new AbortController();
      await Application.login(
        email,
        password,
        rememberMe,
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
      return <ModalLoading text="Выполняется вход" />;
    }

    return (
      <ModalContainer>
        <Modal
          ref={this.formRef}
          autoComplete="off"
          onSubmit={this.onSubmit}
          // onFocus={this.onFocus}
          // onBlur={this.onBlur}
          // onInvalid={this.onInvalid}
        >
          <ModalHeader>Вход</ModalHeader>
          {this.state.error && <ModalError>{this.state.error}</ModalError>}
          <ModalField>
            <div>Электронная почта</div>
            <Input
              autoFocus
              type="email"
              placeholder="Введите электронную почту"
              name="email"
              required
              autoComplete="current-email"
              autoCorrect="off"
              autoCapitalize="none"
              maxLength="128"
              disabled={this.state.loading}
            />
          </ModalField>
          {this.state.emailError && (
            <ModalFieldError>{this.state.emailError}</ModalFieldError>
          )}
          <ModalField>
            <div>Пароль</div>
            <Input
              type="password"
              placeholder="Введите пароль"
              name="password"
              required
              autoComplete="current-password"
              autoCorrect="off"
              autoCapitalize="none"
              pattern=".{8,}"
              title="Минимальная длинна пароля должна составлять 8 символов"
              maxLength="64"
              disabled={this.state.loading}
            />
          </ModalField>
          {this.state.passwordError && (
            <ModalFieldError>{this.state.passwordError}</ModalFieldError>
          )}
          <ModalField direction="row">
            <CheckBox defaultChecked="checked" name="remember" />
            <div>Запомнить компьютер</div>
          </ModalField>
          <ModalActions>
            <Button type="submit" disabled={!this.canSubmit()}>
              Войти
              <LoginIcon />
            </Button>
            <RegistrationLinkContainer>
              <Link to="/registration">Зарегистрироваться</Link>
            </RegistrationLinkContainer>
          </ModalActions>
        </Modal>
      </ModalContainer>
    );
  }
}

const RegistrationLinkContainer = styled.div`
  display: flex;
  align-self: center;
`;
