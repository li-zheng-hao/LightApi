// index.ts
import axios from 'axios'
import type { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios'
import { handleHttpError } from './httpErrorHandler'
import { deepMerge } from '@/utils'
import { handleBusinessError } from '@/api/client/businessErrorHandler'
import { useJwt } from './jwtAuth'
import { generateRequestKey } from './helper'
import idmp, { type IdmpOptions } from 'idmp'

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
  defaultRequestConfig: RequestConfig = {
    showError: true,
    unwrapResult: true,
    returnRawAxiosResponse: false,
    throwBusinessError: true,
    useIdmp: true
  }

  constructor(config: AxiosRequestConfig, requestConfig?: RequestConfig) {
    this.defaultRequestConfig = Object.assign(this.defaultRequestConfig, requestConfig ?? {})
    // 使用axios.create创建axios实例
    this.axiosInstance = axios.create(Object.assign(this.baseConfig, config))

    // 改为使用axios-jwt库进行token的管理
    // this.axiosInstance.interceptors.request.use(
    //     (config: any) => {
    //         // 一般会请求拦截里面加token，用于后端的验证
    //         const token = localStorage.getItem("token") as string
    //         if(token) {
    //             config.headers!.Authorization = token;
    //         }
    //         return config;
    //     },
    //     (err: any) => {
    //         // 请求错误，这里可以用全局提示框进行提示
    //         return Promise.reject(err);
    //     }
    // );

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
    if (targetConfig.useIdmp) {
      const requestKey = generateRequestKey(config)
      return idmp(
        requestKey,
        () => this.internalRequest<T>(config, targetConfig),
        targetConfig.idmpOptions ?? undefined
      )
    }
    return this.internalRequest<T>(config, targetConfig)
  }

  /**
   * 内部调用
   * @param config
   * @param requestConfig
   * @returns
   */
  private internalRequest<T>(config: AxiosRequestConfig, requestConfig: RequestConfig): Promise<T> {
    return new Promise((resolve) => {
      this.axiosInstance
        .request<any, AxiosResponse<ApiResult<T>>>(config)
        .then((res: AxiosResponse<ApiResult<T>>) => {
          if (!res.data.success) handleBusinessError(res.data, requestConfig)
          if (res.data.success && requestConfig.unwrapResult) return resolve(res.data.data)
          else if (requestConfig?.returnRawAxiosResponse) return resolve(res as any)
          else return resolve(res.data as any)
        })
    })
  }
}

// 如果有需要可以配置多个
const apiClient = new ApiClient({})

useJwt(apiClient)

export { apiClient }
