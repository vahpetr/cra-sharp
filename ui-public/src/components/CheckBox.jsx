import React from 'react';
import styled from 'styled-components/macro';

export default function CheckBox(props) {
  return <Container {...props} type="checkbox" />;
}

const Container = styled.input`
  vertical-align: baseline;
  margin: 2px 4px;
`;
