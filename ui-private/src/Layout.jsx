import React from 'react';
import styled from 'styled-components/macro';
import { StyledLayout } from './styles';

export default function Layout(props) {
  return (
    <App>
      <StyledLayout />
      <Header />
      {props.children}
      <Footer />
    </App>
  );
}

const Header = styled.div`
  /* nop */
`;

const Footer = styled.div`
  /* nop */
`;

const App = styled.div`
  height: 100%;
  background-color: whitesmoke;
`;
