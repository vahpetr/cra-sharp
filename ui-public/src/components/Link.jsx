import React from 'react';
import { Link as DomLink } from 'react-router-dom';
import styled from 'styled-components/macro';

export default function Link(props) {
  return <Container {...props} />;
}

const Container = styled(DomLink)`
  font-family: var(--font-serif);
  color: #444;
  display: inline-block;
  cursor: pointer;
  outline: 0;
  padding: 0 2px;
  margin: 0 2px;

  :hover {
    color: #e86400;
    background-color: rgba(240, 240, 240, 0.75);
  }
`;
