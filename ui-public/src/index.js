import React from 'react';
import ReactDOM from 'react-dom';
import Application from './modules/Application';
import App from './App';
import config from './config';
// import * as serviceWorker from 'serviceWorker';

// eslint-disable-next-line no-console
console.log(config);

if (Application.hasAccessToken()) {
  window.location.href = window.location.origin;
}

ReactDOM.render(<App />, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
// serviceWorker.unregister();
