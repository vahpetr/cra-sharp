import React from 'react';
import Application from '../modules/Application';

export default class LogoutPage extends React.Component {
  abortController = new AbortController();

  componentDidMount() {
    this.logout();
  }

  componentWillUnmount() {
    this.abortController.abort();
  }

  async logout() {
    return await Application.logout(this.abortController.signal);
  }

  render() {
    return null;
  }
}
