import React from 'react';
import styled from 'styled-components/macro';

export default function Input(props) {
  return <Container {...props} />;
}

Input.defaultProps = {
  autoFocus: false,
  required: false
};

const Container = styled.input`
  font-family: var(--font-serif);
  font-size: 14px;
  height: 30px;
  padding: 4px;
  border: 1px solid black;
  box-sizing: border-box;
  display: inline-block;
  outline: none;
  max-width: 300px;
  border-radius: 2px;

  :focus {
    border: 1px solid gray;
  }

  :invalid {
    border: 1px solid yellowgreen;
  }

  :required {
    background-color: #fffcee;
  }

  :valid {
    border: 1px solid green;
  }
`;
