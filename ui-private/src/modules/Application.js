import { API_HOST } from '../config';
import HttpTransport from '../transports/HttpTransport';
import LocalStorage from '../modules/LocalStorage';
import DataProvider from '../providers/DataProvider';
import UsersProvider from '../providers/UsersProvider';

const ACCESS_TOKEN = 'ACCESS_TOKEN';

class Application {
  _storage = new LocalStorage();

  constructor() {
    const token = this.storage.getItem(ACCESS_TOKEN);
    this._dataProvider = new DataProvider(new HttpTransport(API_HOST), token);
    this._usersProvider = new UsersProvider(new HttpTransport(API_HOST), token);
  }

  get storage() {
    return this._storage;
  }

  get dataProvider() {
    return this._dataProvider;
  }

  get usersProvider() {
    return this._usersProvider;
  }

  async logout(signal) {
    try {
      await this.usersProvider.logout(signal);
    } catch (error) {
      /* nop */
    }
    this.storage.removeItem(ACCESS_TOKEN);
    window.location.href = `${window.location.origin}/login${window.location.search}`;
  }

  get user() {
    const token = this.storage.getItem(ACCESS_TOKEN);
    if (!token) {
      return null;
    }

    const data = parseJwtToken(token);
    const roleTitleMap = {
      User: 'Пользователь'
    };
    const role =
      data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    return {
      id: data.sub,
      email: data.email,
      role: roleTitleMap[role] || role
    };
  }

  get token() {
    const token = this.storage.getItem(ACCESS_TOKEN);
    if (!token) {
      return null;
    }

    const data = parseJwtToken(token);
    return {
      id: data.jti,
      expires: data.exp
    };
  }
}

function parseJwtToken(auth) {
  const [, base64Url] = auth.split('.');
  const base64 = base64Url.replace('-', '+').replace('_', '/');
  return JSON.parse(atob(base64));
}

const application = new Application();

export default application;
