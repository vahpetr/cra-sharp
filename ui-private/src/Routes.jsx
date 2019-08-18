import React from 'react';
import { Route, Switch, Redirect } from 'react-router';
import DashboardPage from './pages/DashboardPage';
import LogoutPage from './pages/LogoutPage';
import ErrorPage from './pages/ErrorPage';

export default function Routes() {
  return (
    <Switch>
      <Redirect exact from="/" to="/dashboard" />
      <Route exact path="/dashboard" component={DashboardPage} />
      <Route exact path="/logout" component={LogoutPage} />
      <Route component={ErrorPage} />
    </Switch>
  );
}
