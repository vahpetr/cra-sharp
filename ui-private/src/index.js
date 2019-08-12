import React from 'react';
import ReactDOM from 'react-dom';
import Application from './modules/Application';
import App from './App';
// import * as serviceWorker from 'serviceWorker';

if (!Application.hasAccessToken()) {
  window.location.href = `${window.location.origin}/login`;
}

ReactDOM.render(<App />, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
// serviceWorker.unregister();
