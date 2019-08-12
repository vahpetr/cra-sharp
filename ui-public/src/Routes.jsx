import React from 'react';
import { Route, Switch, Redirect } from 'react-router';
import LoginPage from './pages/LoginPage';
import RegistrationPage from './pages/RegistrationPage';
import RegistrationSuccessPage from './pages/RegistrationSuccessPage';
import RegistrationActivationPage from './pages/RegistrationActivationPage';
import ErrorPage from './pages/ErrorPage';

export default function Routes() {
  return (
    <Switch>
      <Redirect exact from="/" to="/login" />
      <Route exact path="/login" component={LoginPage} />
      <Route exact path="/registration" component={RegistrationPage} />
      <Route
        exact
        path="/registration/success"
        component={RegistrationSuccessPage}
      />
      <Route
        exact
        path="/registration/activation/:activationToken"
        component={RegistrationActivationPage}
      />
      <Route component={ErrorPage} />
    </Switch>
  );
}
