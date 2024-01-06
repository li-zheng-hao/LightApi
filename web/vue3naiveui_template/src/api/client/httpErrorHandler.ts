import router from "@/router";
import type { AxiosResponse } from "axios";

export function handleHttpError(error: AxiosResponse | undefined) {

    if (!error) return
    // 这里用来处理http常见错误，进行全局提示
    let message = "";
    switch (error.status) {
        case 400:
            message = "请求错误(400)";
            break;
        case 401:
            message = "未授权，请重新登录(401)";
            // 这里可以做清空storage并跳转到登录页的操作
            localStorage.clear()
            router.push('/login')
            break;
        case 403:
            message = "权限不足(403)";
            break;
        case 404:
            message = "请求出错(404)";
            break;
        case 408:
            message = "请求超时(408)";
            break;
        case 500:
            message = "服务器错误(500)";
            break;
        case 501:
            message = "服务未实现(501)";
            break;
        case 502:
            message = "网络错误(502)";
            break;
        case 503:
            message = "服务不可用(503)";
            break;
        case 504:
            message = "网络超时(504)";
            break;
        case 505:
            message = "HTTP版本不受支持(505)";
            break;
        case 200:
            message = error.data.msg;
            break;
        default:
            message = `操作失败(${error.status})`;
    }
    if(error.status>=400){
        message+="，请检查网络或者联系管理员"
    }
    window['$message'].error(`${message}`, { duration: 6000, closable: true })
}