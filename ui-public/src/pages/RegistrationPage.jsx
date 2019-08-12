import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components/macro';
import Application from '../modules/Application';
import Input from '../components/Input';
import Button from '../components/Button';
import ModalLoading from '../components/ModalLoading';
import Link from '../components/Link';
import {
  Modal,
  ModalActions,
  ModalField,
  ModalHeader,
  ModalContainer,
  ModalError,
  ModalFieldError
} from '../components/Modal';

export default class RegistrationPage extends React.Component {
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
    repeatError: '',
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
      const { email, password, repeat } = this.formRef.current;
      validateEmail(email);
      validatePassword(password, repeat);

      this.setState({
        passwordError: password.validationMessage,
        repeatError: repeat.validationMessage,
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

  onSubmit = async e => {
    e.preventDefault();

    const { email, password, repeat } = e.target;
    validatePassword(password, repeat);

    const isValid = this.formRef.current.checkValidity();
    this.setState({
      emailError: email.validationMessage,
      passwordError: password.validationMessage,
      repeatError: repeat.validationMessage,
      validated: true,
      isValid
    });

    if (isValid) {
      this.registration(email.value, repeat.value);
    }
  };

  canSubmit = () => {
    if (this.state.loading) {
      return false;
    }
    const { validated, isValid } = this.state;
    return !validated || isValid;
  };

  async registration(email, password) {
    try {
      this.abortController.abort();
      this.setState({ loading: true });
      this.abortController = new AbortController();
      await Application.usersProvider.registration(
        email,
        password,
        this.abortController.signal
      );
      this.props.history.push('/registration/success');
    } catch (error) {
      // eslint-disable-next-line no-console
      console.error(error);
      this.setState({
        loading: false,
        error: error.message
      });
    }
  }

  render() {
    if (this.state.loading) {
      return <ModalLoading text="Выполняется регистрация" />;
    }

    return (
      <ModalContainer>
        <Modal
          ref={this.formRef}
          // novalidate
          autoComplete="off"
          onSubmit={this.onSubmit}
          // onInvalid={this.onInvalid}
          // onFocus={this.onFocus}
          // onBlur={this.onBlur}
        >
          <ModalHeader>Регистрация</ModalHeader>
          {this.state.error && <ModalError>{this.state.error}</ModalError>}
          <ModalField>
            <div>Электронная почта</div>
            <Input
              autoFocus
              type="email"
              placeholder="Введите электронную почту"
              name="email"
              required
              autoComplete="new-email"
              autoCorrect="off"
              autoCapitalize="none"
              maxLength="128"
              onChange={this.onEmailChange}
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
              autoComplete="new-password"
              autoCorrect="off"
              autoCapitalize="none"
              pattern=".{8,}"
              title="Минимальная длинна пароля должна составлять 8 символов"
              maxLength="64"
              onChange={this.onPasswordChange}
              disabled={this.state.loading}
            />
          </ModalField>
          {this.state.passwordError && (
            <ModalFieldError>{this.state.passwordError}</ModalFieldError>
          )}
          <ModalField>
            <div>Повтор пароля</div>
            <Input
              type="password"
              placeholder="Введите пароль повторно"
              name="repeat"
              required
              autoComplete="off"
              autoCorrect="off"
              autoCapitalize="none"
              pattern=".{8,}"
              title="Минимальная длинна пароля должна составлять 8 символов"
              maxLength="64"
              onChange={this.onPasswordChange}
              disabled={this.state.loading}
            />
          </ModalField>
          {this.state.repeatError && (
            <ModalFieldError>{this.state.repeatError}</ModalFieldError>
          )}
          <ModalActions>
            <Button type="submit" disabled={!this.canSubmit()}>
              Зарегистрироваться
            </Button>
            <BackLinkContainer>
              <Link to="/">На страницу входа</Link>
            </BackLinkContainer>
          </ModalActions>
        </Modal>
      </ModalContainer>
    );
  }
}

function validateEmail(email) {
  /* nop */
}

function validatePassword(password, repeat) {
  if (password.value !== repeat.value) {
    repeat.setCustomValidity('Пароли не совпадают');
  } else {
    repeat.setCustomValidity('');
  }
}

const BackLinkContainer = styled.div`
  display: flex;
  align-self: center;
`;
