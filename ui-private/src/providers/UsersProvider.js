const META_DATA_HEADERS = {
  'x-request-client-type': 'users-provider',
  'x-request-client-version': '1.0'
};

const JSON_HEADERS = {
  'content-type': 'application/json'
};

export default class UsersProvider {
  _transport = null;
  _token = null;

  constructor(transport, token) {
    if (!transport) {
      throw new TypeError('Transport not set.');
    }

    this._transport = transport;
    this._token = token;
  }

  secureHeaders() {
    return {
      ...jsonHeaders(),
      Authorization: `Bearer ${this._token}`
    };
  }

  async logout(signal) {
    return await this._transport.post('/api/users/logout', {
      headers: this.secureHeaders(),
      credentials: 'same-origin',
      signal
    });
  }
}

function infoHeaders() {
  return {
    ...META_DATA_HEADERS,
    'x-request-id': Date.now()
  };
}

function jsonHeaders() {
  return {
    ...JSON_HEADERS,
    ...infoHeaders()
  };
}
