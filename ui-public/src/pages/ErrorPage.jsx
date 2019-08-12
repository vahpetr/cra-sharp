import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components/macro';
import Button from '../components/Button';

export default class ErrorPage extends React.Component {
  static propTypes = {
    history: PropTypes.object.isRequired
  };
  render() {
    return (
      <StyledContainer>
        <div>
          <StyledMessage>Старница не найденна</StyledMessage>
          <Button onClick={this.props.history.goBack}>Вернуться назад</Button>
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
