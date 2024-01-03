import { InternalAxiosRequestConfig, AxiosResponse } from 'axios'

// 业务接口结构
interface ApiResult<T = unknown> extends AxiosResponse {
  code: number
  data: T
  msg: string
  success:boolean
  response?: AxiosResponse
}

interface ApiRequestConfig extends InternalAxiosRequestConfig {
  // 用户自定义配置 是否静默不提示错误
  silent?: boolean
}

export type { ApiResult, ApiRequestConfig }
