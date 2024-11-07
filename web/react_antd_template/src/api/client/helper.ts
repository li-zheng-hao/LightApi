import type { AxiosRequestConfig } from 'axios';

/**
 * 根据axios请求生成对应的请求key
 * @param config
 * @returns
 */
export function generateRequestKey(config: AxiosRequestConfig) {
  const { method, url, params, data } = config;
  return [method, url, JSON.stringify(params), JSON.stringify(data)].join('&');
}
