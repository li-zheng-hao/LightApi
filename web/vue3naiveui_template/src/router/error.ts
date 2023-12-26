export function getErrorRoutes() {
    return [{
        path: '/error/404',
        name: '无数据',
        component: () => import('../components/error/NotFoundPage.vue')
    },
        {
            path: '/error/403',
            name: '无权限',
            component: () => import('../components/error/NoPermissionPage.vue')
        },
        {
            path: '/error/500',
            name: '系统错误',
            component: () => import('../components/error/ErrorPage.vue')
        }]
}
