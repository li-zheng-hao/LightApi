import { router } from "@/router";
import { isEmpty } from "@/utils/common/isEmpty";
import { message } from "antd";
import type { AxiosResponse } from "axios";

export function handleHttpError(error: AxiosResponse | undefined) {
    if (!error) return
    // 这里用来处理http常见错误，进行全局提示
    let errorMsg = "";
    switch (error.status) {
        case 400:
            errorMsg = "请求错误(400)";
            break;
        case 401:
            // 这里可以做清空storage并跳转到登录页的操作
            localStorage.clear()
            router.navigate("/login")
            break;
        case 403:
            errorMsg = "权限不足(403)";
            break;
        case 404:
            errorMsg = "请求出错(404)";
            break;
        case 408:
            errorMsg = "请求超时(408)";
            break;
        case 500:
            errorMsg = "服务器错误(500)";
            break;
        case 501:
            errorMsg = "服务未实现(501)";
            break;
        case 502:
            errorMsg = "网络错误(502)";
            break;
        case 503:
            errorMsg = "服务不可用(503)";
            break;
        case 504:
            errorMsg = "网络超时(504)";
            break;
        case 505:
            errorMsg = "HTTP版本不受支持(505)";
            break;
        case 200:
            errorMsg = error.data.msg;
            break;
        default:
            errorMsg = `操作失败(${error.status})`;
    }
    if(error.status>=500){
        errorMsg+="，请检查网络或者联系管理员"
    }
    if(!isEmpty(errorMsg))
        message.error({content:`${errorMsg}`, duration: 6000})
}