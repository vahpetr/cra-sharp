import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components/macro';
import Button from '../components/Button';

export default class ErrorPage extends React.Component {
  static propTypes = {
    history: PropTypes.shape({
      push: PropTypes.func.isRequired
    }).isRequired
  };

  onLogin = () => this.props.history.push('/login');

  render() {
    return (
      <StyledContainer>
        <div>
          <StyledMessage>В доступе отказано</StyledMessage>
          <Button onClick={this.onLogin}>Вернуться на страницу входа</Button>
        </div>
      </StyledContainer>
    );
  }
}

const StyledMessage = styled.h1`
  margin-top: 0;
  width: 450px;
`;

const StyledContainer = styled.div`
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  text-align: center;
`;
