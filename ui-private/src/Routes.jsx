import React from 'react';
import { Route, Switch, Redirect } from 'react-router';
import DashboardPage from './pages/DashboardPage';
import ErrorPage from './pages/ErrorPage';

export default function Routes() {
  return (
    <Switch>
      <Redirect exact from="/" to="/dashboard" />
      <Route exact path="/dashboard" component={DashboardPage} />

      <Route component={ErrorPage} />
    </Switch>
  );
}
