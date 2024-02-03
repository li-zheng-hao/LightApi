import {ApiResult, RequestConfig} from "./apiClient.ts";
import {message} from "@/utils/message.ts";

/**
 * 处理业务异常
 * @param apiResult
 * @param requestConfig
 */
export function handleBusinessError(apiResult: ApiResult<unknown> , requestConfig:RequestConfig) {
    if(apiResult?.msg.length>0&&requestConfig.showError) {
        message.error(apiResult.msg, 6);
    }
    if(requestConfig.throwBusinessError)
        throw new Error(apiResult?.msg??"操作失败")
}
