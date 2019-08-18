const META_DATA_HEADERS = {
  'x-request-client-type': 'data-provider',
  'x-request-client-version': '1.0'
};

const JSON_HEADERS = {
  'content-type': 'application/json'
};

export default class DataProvider {
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

  async getData(signal) {
    return await this._transport.get('/api/data', {
      headers: this.secureHeaders(),
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
