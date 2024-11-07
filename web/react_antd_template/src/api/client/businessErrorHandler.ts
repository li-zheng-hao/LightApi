import type { ApiResult, RequestConfig } from '@/api/client/apiClient';
import { message } from 'antd';

/**
 * 处理业务异常
 * @param apiResult
 * @param reqeustConfig
 */
export function handleBusinessError(
  apiResult: ApiResult<any>,
  requestConfig: RequestConfig
) {
  if (apiResult?.msg.length > 0 && requestConfig.showError) {
    message.error({ duration: 6000, content: `${apiResult.msg}` });
  }
  if (requestConfig.throwBusinessError)
    throw new Error(apiResult?.msg ?? '操作失败');
}
