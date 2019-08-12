import React from 'react';
import styled from 'styled-components/macro';

export default function Button(props) {
  return <Container {...props} />;
}

Button.defaultProps = {
  type: 'button',
  active: false
};

const Container = styled.button.attrs(props => ({
  'data-active': props.active
}))`
  font-family: var(--font-serif);
  padding: 8px 16px;
  color: #fff;
  background-color: #009688;
  text-align: center;
  text-decoration: none;
  font-size: 1em;
  cursor: pointer;
  display: inline-block;
  outline: 0;
  user-select: none;
  border: none;
  white-space: nowrap;
  overflow: hidden;
  vertical-align: middle;
  border-radius: 2px;

  :hover {
    color: #000;
    background-color: #fdcf;
  }

  & + & {
    margin-left: 4px;
  }
`;
