export const STATUS_UNAUTHORIZED = 401;
export const STATUS_FORBIDDEN = 403;

export default class HttpTransport {
  _host = null;

  constructor(host) {
    if (!host) {
      throw new TypeError('Host not set.');
    }

    this._host = host;
  }

  async request(url, config) {
    const response = await fetch(`${this._host}${url}`, config);
    switch (response.status) {
    case STATUS_UNAUTHORIZED:
    case STATUS_FORBIDDEN:
      logout(config);
      break;
    default:
      if (
        config.headers.accept &&
          config.headers.accept.includes('application/json')
      ) {
        const data = await response.json();
        if (data.error) {
          throw new Error(
            data.message || data.error || 'Неизвестная ошибка.'
          );
        }

        return data;
      }

      return await response.text();
    }
  }

  get(url, config) {
    return this.request(url, {
      ...config,
      method: 'GET'
    });
  }

  post(url, config) {
    return this.request(url, {
      ...config,
      method: 'POST'
    });
  }

  delete(url, config) {
    return this.request(url, {
      ...config,
      method: 'DELETE'
    });
  }
}

function logout(config) {
  if (config.headers.authorization) {
    const { pathname } = window.location;
    if (!['/login', '/logout'].includes(pathname)) {
      const { origin, search, hash } = window.location;
      const returnUrl = encodeURIComponent(pathname + search + hash);
      window.location.href = `${origin}/logout?returnUrl=${returnUrl}`;
    }
  }
}
