import React from 'react';
import ReactDOM from 'react-dom';
import ErrorPage from '../pages/ErrorPage';

it('ErrorPage renders without crashing', () => {
  const div = document.createElement('div');
  ReactDOM.render(<ErrorPage history={{goBack: () => null}}/>, div);
  ReactDOM.unmountComponentAtNode(div);
});
