export function getExampleRoutes() {
  return [
    {
      path: '/example/button/button1',
      name: 'ButtonExample1',
      component: () => import('@/views/ButtonExample1.vue')
    },
    {
      path: '/example/button/button2',
      name: 'ButtonExample2',
      component: () => import('@/views/ButtonExample2.vue')
    },
    {
      path: '/example/table',
      name: 'TableExample',
      component: () => import('@/views/TableExample.vue')
    },
    {
      path: '/example/xtable',
      name: 'XTableExample',
      component: () => import('@/views/XTableExample.vue')
    },
    {
      path: '/example/echarts',
      name: 'EchartsExample',
      component: () => import('@/views/EchartsExample.vue')
    }
  ]
}
