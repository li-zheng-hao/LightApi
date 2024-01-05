export function getResultRoutes() {
  return [
    {
      path: '/result/success',
      name: 'SuccessResult',
      component: () => import('@/components/result/SuccessResult.vue')
    },
    {
      path: '/result/fail',
      name: 'FailResult',
      component: () => import('@/components/result/FailResult.vue')
    },
    {
      path: '/result/info',
      name: 'InfoResult',
      component: () => import('@/components/result/InfoResult.vue')
    }
  ]
}
