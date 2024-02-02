import type {ApiResult, RequestConfig} from "@/api/client/apiClient";

/**
 * 处理业务异常
 * @param apiResult
 * @param reqeustConfig
 */
export function handleBusinessError(apiResult: ApiResult<unknown> , requestConfig:RequestConfig) {
    if(apiResult?.msg.length>0&&requestConfig.showError) {
        // window['$message'].error(`${apiResult.msg}`, { duration: 6000, closable: true })
    }
    if(requestConfig.throwBusinessError)
        throw new Error(apiResult?.msg??"操作失败")
}
