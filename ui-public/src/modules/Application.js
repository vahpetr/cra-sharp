import { API_HOST } from '../config';
import HttpTransport from '../transports/HttpTransport';
import UsersProvider from '../providers/UsersProvider';
import LocalStorage from '../modules/LocalStorage';

const ACCESS_TOKEN = 'ACCESS_TOKEN';

class Application {
  _usersProvider = new UsersProvider(new HttpTransport(API_HOST));
  _storage = new LocalStorage();

  get usersProvider() {
    return this._usersProvider;
  }

  get storage() {
    return this._storage;
  }

  async registrationConfirm(activationToken, signal) {
    const { accessToken } = await this.usersProvider.registrationConfirm(
      activationToken,
      signal
    );
    this.storage.setItem(ACCESS_TOKEN, accessToken);
  }

  async login(login, password, signal) {
    const { accessToken } = await this.usersProvider.login(
      login,
      password,
      signal
    );
    this.storage.setItem(ACCESS_TOKEN, accessToken);
  }
}

const application = new Application();

export default application;
