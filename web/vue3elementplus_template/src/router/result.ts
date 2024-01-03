export function getResultRoutes() {
    return [{
        path: '/result/success',
        name: '成功',
        component: () => import('../components/error/NotFoundPage.vue')
    },
        {
            path: '/result/fail',
            name: '失败',
            component: () => import('../components/error/NoPermissionPage.vue')
        },
        {
            path: '/result/info',
            name: '信息',
            component: () => import('../components/error/ErrorPage.vue')
        }]
}
