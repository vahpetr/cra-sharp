import React from 'react';
import { createBrowserHistory } from 'history';
import { Router } from 'react-router';
import Layout from './Layout';
import Routes from './Routes';

const createAppHistory = () => {
  const history = createBrowserHistory();

  if (process.env.NODE_ENV === 'development') {
    history.listen((location, action) => {
      // eslint-disable-next-line no-console
      console.log(action, location.pathname, location.state);
    });
  }

  return history;
};

const history = createAppHistory();

export default function App() {
  return (
    <Router history={history}>
      <Layout>
        <Routes />
      </Layout>
    </Router>
  );
}
