// index.ts
import axios from "axios";
import type { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { handleError } from "./logHandler";
import type { ApiResult } from "../common";

type Result<T> = {
    code: number;
    msg: string;
    data: T;
    success: boolean;
};

// 导出Request类，可以用来自定义传递配置来创建实例
export class Request {
    // axios 实例
    instance: AxiosInstance;
    // 基础配置，url和超时时间
    baseConfig: AxiosRequestConfig = { baseURL: "/api", timeout: 60000 };

    constructor(config: AxiosRequestConfig) {
        // 使用axios.create创建axios实例
        this.instance = axios.create(Object.assign(this.baseConfig, config));

        this.instance.interceptors.request.use(
            (config: any) => {
                // 一般会请求拦截里面加token，用于后端的验证
                const token = localStorage.getItem("token") as string
                if(token) {
                    config.headers!.Authorization = token;
                }

                return config;
            },
            (err: any) => {
                // 请求错误，这里可以用全局提示框进行提示
                return Promise.reject(err);
            }
        );

        this.instance.interceptors.response.use(
            (res: AxiosResponse) => {
                // 这里可以对响应数据进行处理，比如后端的错误提示
                if(res.data?.success===false){
                    handleError(res)
                    return Promise.reject(res.data);
                }
                return res;
            },
            (err: AxiosError) => {
                handleError(err.response)

                return Promise.reject(err);
            }
        );
    }

    // 定义请求方法
    public request(config: AxiosRequestConfig): Promise<AxiosResponse> {
        return this.instance.request(config);
    }

    public get<T = any>(
        url: string,
        config?: AxiosRequestConfig
    ): Promise<AxiosResponse<Result<T>>> {
        return this.instance.get(url, config);
    }

    public post<T = any>(
        url: string,
        data?: any,
        config?: AxiosRequestConfig
    ): Promise<AxiosResponse<Result<T>>> {
        return this.instance.post(url, data, config);
    }

    public put<T = any>(
        url: string,
        data?: any,
        config?: AxiosRequestConfig
    ): Promise<AxiosResponse<Result<T>>> {
        return this.instance.put(url, data, config);
    }

    public delete<T = any>(
        url: string,
        config?: AxiosRequestConfig
    ): Promise<AxiosResponse<Result<T>>> {
        return this.instance.delete(url, config);
    }
}

// 默认导出Request实例
const apiClient= new Request({})
export{ apiClient}
