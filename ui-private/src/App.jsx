import React from 'react';
import { createBrowserHistory } from 'history';
import { Router } from 'react-router';
import { createStore } from 'redux';
import { Provider } from 'react-redux';
import { rootReducer } from './store/rootReducer';
import { setUser } from './store/user';
import Application from './modules/Application';
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

const createAppStore = () => {
  if (process.env.NODE_ENV === 'development') {
    const store = createStore(
      rootReducer,
      window.__REDUX_DEVTOOLS_EXTENSION__ &&
        window.__REDUX_DEVTOOLS_EXTENSION__()
    );

    store.subscribe(() => {
      // eslint-disable-next-line no-console
      console.log('STORE_CHANGE', store.getState());
    });

    return store;
  } else if (process.env.NODE_ENV === 'production') {
    return createStore(rootReducer);
  }
};

export default class App extends React.Component {
  history = createAppHistory();
  store = createAppStore();

  componentDidMount() {
    const action = setUser(Application.user);
    this.store.dispatch(action);
  }

  render() {
    return (
      <Provider store={this.store}>
        <Router history={this.history}>
          <Layout>
            <Routes />
          </Layout>
        </Router>
      </Provider>
    );
  }
}
