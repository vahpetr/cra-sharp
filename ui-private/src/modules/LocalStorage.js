export default class LocalStorage {
  _storage = window.localStorage;

  getItem(keyName) {
    return this._storage.getItem(keyName);
  }

  setItem(keyName, keyValue) {
    return this._storage.setItem(keyName, keyValue);
  }

  removeItem(keyName) {
    return this._storage.removeItem(keyName);
  }

  clear() {
    return this._storage.clear();
  }
}
