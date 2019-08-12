import React from 'react';
import PropTypes from 'prop-types';
import styled, { keyframes } from 'styled-components/macro';

export default function Loading(props) {
  return <Container>{props.text}</Container>;
}

Loading.propTypes = {
  text: PropTypes.string
};

Loading.defaultProps = {
  text: 'Загрузка'
};

const DotsAnimation = keyframes`
  0% {
    opacity: 0;
  }

  25% {
    opacity: 1;
  }

  50% {
    text-shadow: 0.5em 0;
  }

  75% {
    text-shadow: 0.5em 0, 1em 0;
  }
`;

const Container = styled.span`
  padding: 0 24px 0 0;

  ::after {
    content: ' .';
    animation: ${DotsAnimation} 3s steps(1, end) infinite;
  }
`;
