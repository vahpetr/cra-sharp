const META_DATA_HEADERS = {
  'x-request-client-type': 'users-provider',
  'x-request-client-version': '1.0'
};

const JSON_HEADERS = {
  accept: 'application/json',
  'content-type': 'application/json;charset=UTF-8'
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
    await this._transport.post('/api/users/registration', {
      body: JSON.stringify({ email, password }),
      headers: jsonHeaders(),
      signal
    });
  }

  async registrationConfirm(activationToken, signal) {
    const authorization = await this._transport.post(
      '/api/users/registration/confirm',
      {
        body: JSON.stringify({ activationToken }),
        headers: jsonHeaders(),
        signal
      }
    );
    return Object.assign(authorization, {
      user: mapUser(parseJwtToken(authorization.accessToken))
    });
  }

  async login(email, password, signal) {
    const authorization = await this._transport.post('/api/users/login', {
      body: JSON.stringify({ email, password }),
      headers: jsonHeaders(),
      signal
    });
    return Object.assign(authorization, {
      user: mapUser(parseJwtToken(authorization.accessToken))
    });
  }
}

function parseJwtToken(auth) {
  const [, base64Url] = auth.split('.');
  const base64 = base64Url.replace('-', '+').replace('_', '/');
  return JSON.parse(atob(base64));
}

function mapUser(data) {
  return {};
}

function textHeaders() {
  return {
    ...META_DATA_HEADERS,
    'x-request-id': Date.now()
  };
}

function jsonHeaders() {
  return {
    ...JSON_HEADERS,
    ...textHeaders()
  };
}
