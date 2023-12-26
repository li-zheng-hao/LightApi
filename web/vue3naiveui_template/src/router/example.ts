export function getExampleRoutes() {
    return [{
        path: '/example/button/button1',
        name: 'Button示例1',
        component: () => import('../views/ButtonExample1.vue')
    },
        {
            path: '/example/button/button2',
            name: 'Button示例2',
            component: () => import('../views/ButtonExample2.vue')
        }
    ]
}
