import React from 'react';
import ReactDOM from 'react-dom';
import Button from '../components/Button';

it('Button renders without crashing', () => {
  const div = document.createElement('div');
  ReactDOM.render(<Button />, div);
  ReactDOM.unmountComponentAtNode(div);
});
