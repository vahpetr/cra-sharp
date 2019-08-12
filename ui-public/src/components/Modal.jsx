import styled from 'styled-components/macro';

export const ModalFieldError = styled.div.attrs({
  'aria-live': 'polite'
})`
  padding: 8px;
  color: #a94442;
  background-color: #f2dede;
  border-color: #ebccd1;
`;

export const ModalError = styled(ModalFieldError)`
  margin-bottom: 8px;
`;

export const ModalField = styled.label`
  display: flex;
  flex-direction: ${({ direction = 'column' }) => direction};

  & + & {
    padding-top: 24px;
  }
`;

export const Modal = styled.form`
  background-color: white;
  border: 1px solid whitesmoke;
  padding: 0 24px;
  max-width: 370px;
  border-radius: 4px;
`;

export const ModalActions = styled.div`
  display: flex;
  justify-content: space-evenly;
  padding: 24px 0 16px 0;
`;

export const ModalHeader = styled.h3`
  display: flex;
  justify-content: center;
  margin: 1em;
`;

export const ModalContainer = styled.div`
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
`;
