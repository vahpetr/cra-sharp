const META_DATA_HEADERS = {
  'x-request-client-type': 'users-provider',
  'x-request-client-version': '1.0'
};

const JSON_HEADERS = {
  'content-type': 'application/json'
};

export default class UsersProvider {
  _transport = null;

  constructor(transport) {
    if (!transport) {
      throw new TypeError('Transport not set.');
    }

    this._transport = transport;
  }

  async registration(email, password, signal) {
    return await this._transport.post('/api/users/registration', {
      body: JSON.stringify({ email, password }),
      headers: jsonHeaders(),
      signal
    });
  }

  async registrationConfirm(activationToken, signal) {
    return await this._transport.post('/api/users/registration/confirm', {
      body: JSON.stringify({ activationToken }),
      headers: jsonHeaders(),
      credentials: 'same-origin',
      signal
    });
  }

  async login(email, password, rememberMe, signal) {
    return await this._transport.post('/api/users/login', {
      body: JSON.stringify({ email, password, rememberMe }),
      headers: jsonHeaders(),
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
