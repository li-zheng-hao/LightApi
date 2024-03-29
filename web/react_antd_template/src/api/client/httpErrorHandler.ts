import type { AxiosResponse } from "axios";
import {message} from "@/utils/message.ts";
export function handleHttpError(error: AxiosResponse | undefined) {
    // navigator

    if (!error) return
    // 这里用来处理http常见错误，进行全局提示
    let errMsg = "";
    switch (error.status) {
        case 400:
            errMsg = "请求错误(400)";
            break;
        case 401:
            errMsg = "未授权，请重新登录(401)";
            // 这里可以做清空storage并跳转到登录页的操作
            localStorage.clear()
            window
                .location.href = "/login"
            break;
        case 403:
            errMsg = "权限不足(403)";
            break;
        case 404:
            errMsg = "请求出错(404)";
            break;
        case 408:
            errMsg = "请求超时(408)";
            break;
        case 500:
            errMsg = "服务器错误(500)";
            break;
        case 501:
            errMsg = "服务未实现(501)";
            break;
        case 502:
            errMsg = "网络错误(502)";
            break;
        case 503:
            errMsg = "服务不可用(503)";
            break;
        case 504:
            errMsg = "网络超时(504)";
            break;
        case 505:
            errMsg = "HTTP版本不受支持(505)";
            break;
        case 200:
            errMsg = error.data.msg;
            break;
        default:
            errMsg = `操作失败(${error.status})`;
    }
    if(error.status>=400){
        errMsg+="，请检查网络或者联系管理员"
    }
    message.error(errMsg, 6);
    // window['$errMsg'].error(`${errMsg}`, { duration: 6000, closable: true ,keepAliveOnHover:true})
}
