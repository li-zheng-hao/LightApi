// index.ts
import axios from 'axios'
import type { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios'
import { handleHttpError } from './httpErrorHandler'
import { generateRequestKey } from './helper'
import idmp, { type IdmpOptions } from 'idmp'
import {handleBusinessError} from "./businessErrorHandler.ts";
import {deepMerge} from "../../utils";

export type ApiResult<T> = {
  code: number
  msg: string
  data: T
  success: boolean
}

export interface RequestConfig {
  /**
   * 弹窗展示异常
   */
  showError?: boolean | undefined | null
  /**
   * 直接返回响应的data数据
   */
  unwrapResult?: boolean | undefined | null
  /**
   * 是否返回原始的Axios响应
   */
  returnRawAxiosResponse?: boolean | undefined | null
  /**
   * 是否抛出业务异常错误
   */
  throwBusinessError?: boolean | undefined | null

  /**
   * 是否使用idmp包裹请求 用于处理幂等和防重复
   */
  useIdmp?: boolean | undefined | null

  /**
   * 是否在请求之前刷新idmp请求key
   */
  refreshIdmpRequestKey?: boolean | undefined | null

  /**
   * idmp相关配置
   */
  idmpOptions?: IdmpOptions | undefined | null
}
export class ApiClient {
  /**
   * axios 实例
   */
  axiosInstance: AxiosInstance
  /**
   * 基础配置，url和超时时间
   */
  baseConfig: AxiosRequestConfig = { baseURL: '/api', timeout: 60000 }
  /**
   * 默认请求配置
   */
  defaultRequestConfig: RequestConfig = {}

  constructor(config: AxiosRequestConfig, requestConfig?: RequestConfig) {
    this.defaultRequestConfig = Object.assign(this.defaultRequestConfig, requestConfig ?? {})
    // 使用axios.create创建axios实例
    this.axiosInstance = axios.create(Object.assign(this.baseConfig, config))

    this.axiosInstance.interceptors.response.use(undefined, (err: AxiosError) => {
      handleHttpError(err.response)
      return Promise.reject(err)
    })
  }

  public getAxios(): AxiosInstance {
    return this.axiosInstance
  }

  /**
   * 请求
   * @param config axios配置
   * @param requestConfig 自定义请求配置
   */
  public request<T>(config: AxiosRequestConfig, requestConfig?: RequestConfig): Promise<T> {
    const targetConfig = deepMerge<RequestConfig>(this.defaultRequestConfig, requestConfig)

    let requestKey = null
    // 如果需要刷新idmp请求key
    if (targetConfig.refreshIdmpRequestKey) {
      requestKey ??= generateRequestKey(config)
      idmp.flush(requestKey)
    }
    // 使用idmp请求
    if (targetConfig.useIdmp) {
      requestKey ??= generateRequestKey(config)
      return idmp(
        requestKey,
        () => this.internalRequest<T>(config, targetConfig),
        targetConfig.idmpOptions ?? undefined
      )
    }
    // 直接使用axios请求
    return this.internalRequest<T>(config, targetConfig)
  }

  /**
   * 内部调用
   * @param config
   * @param requestConfig
   * @returns
   */
  private internalRequest<T>(config: AxiosRequestConfig, requestConfig: RequestConfig): Promise<T> {
    return new Promise((resolve, reject) => {
      this.axiosInstance
        .request<never, AxiosResponse<ApiResult<T>>>(config)
        .then(
          (res: AxiosResponse<ApiResult<T>>) => {
            if (!res.data.success) handleBusinessError(res.data, requestConfig)
            if (res.data.success && requestConfig.unwrapResult) return resolve(res.data.data)
            else if (requestConfig?.returnRawAxiosResponse) return resolve(res as never)
            else return resolve(res.data as never)
          },
          (err) => reject(err)
        )
        .catch((err) => reject(err))
    })
  }
}

// 如果有需要可以配置多个
const apiClient = new ApiClient(
  {},
  {
    showError: true,
    unwrapResult: true,
    returnRawAxiosResponse: false,
    throwBusinessError: true,
    useIdmp: true,
    idmpOptions: {
      maxRetry: 0,
      maxAge: 1000
    }
  }
)

export { apiClient }
