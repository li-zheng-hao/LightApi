import axios from 'axios' // 引入axios
import type { AxiosInstance, AxiosResponse, AxiosError } from 'axios'
import type { ApiResult, ApiRequestConfig } from '../common'
import router from '@/router/index'
import nprogress from "@/utils/nprogress";

class Service {
  service: AxiosInstance
  constructor(baseURL = '/', timeout = 30000) {
    this.service = axios.create({
      baseURL,
      timeout,
      headers: {
        'Content-type': 'application/json'
      }
    })
    this.interceptors()
  }
  interceptors() {
    // http request 拦截器
    this.service.interceptors.request.use(
      (config: ApiRequestConfig) => {
        nprogress.start()
        return config
      },
      (error: AxiosError) => {
        // TODO 请求错误
        // ElMessage({
        //   showClose: true,
        //   message: error.message,
        //   type: 'error'
        // })
        return error
      }
    )
    // http response 拦截器
    this.service.interceptors.response.use(
      (response: AxiosResponse) => {
        nprogress.done()
        const resData = response.data as ApiResult
        if (resData.code === 0) {
          return { data: resData.data, response } as any
        } else {
          const { silent } = response.config as ApiRequestConfig

              // TODO 请求错误
            // ElMessage({
            //   showClose: true,
            //   message: response.data.msg || decodeURI(response.headers.msg),
            //   type: 'error'
            // })
          if (response.data.data && response.data.data.reload) {
            localStorage.clear()
            router.push({ name: 'login', replace: true })
          }
          return { error: true, response }
        }
      },
      (error: AxiosError) => {
        let message = ''
        nprogress.done()

        switch (error.response.status) {
          case 400:
            message = '请求错误(400)'
            break
          case 401:
            message = '未授权，请重新登录(401)'
            localStorage.clear()
            router.push({ name: 'login', replace: true })
            break
          case 403:
            message = '拒绝访问(403)'
            break
          case 404:
            message = '请求出错(404)'
            break
          case 408:
            message = '请求超时(408)'
            break
          case 500:
            message = '服务器错误(500)'
            break
          case 501:
            message = '服务未实现(501)'
            break
          case 502:
            message = '网络错误(502)'
            break
          case 503:
            message = '服务不可用(503)'
            break
          case 504:
            message = '网络超时(504)'
            break
          case 505:
            message = 'HTTP版本不受支持(505)'
            break
          default:
            message = `连接出错(${error.response.status})!`
        }
        // // TODO 这里错误消息可以使用全局弹框展示出来
        // ElMessage({
        //   showClose: true,
        //   message: `${message}，请检查网络或联系管理员！`,
        //   type: 'error'
        // })
        return {
          error: true,
          response: error.response
        }
      }
    )
  }
  get<T = any, D = any>(
    url: string,
    data?: D,
    options?: Partial<ApiRequestConfig>
  ): Promise<ApiResult<T>> {
    return this.service.get(url, { params: data, ...options })
  }
  put<T = any, D = any>(
    url: string,
    data?: D,
    options?: Partial<ApiRequestConfig>
  ): Promise<ApiResult<T>> {
    return this.service.put(url, data, options)
  }
  post<T = any, D = any>(
    url: string,
    data?: D,
    options?: Partial<ApiRequestConfig>
  ): Promise<ApiResult<T>> {
    return this.service.post(url, data, options)
  }
  delete<T = any>(
    url: string,
    options?: Partial<ApiRequestConfig>
  ): Promise<ApiResult<T>> {
    return this.service.delete(url, options)
  }
}

export default new Service()
