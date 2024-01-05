export function getErrorRoutes() {
  return [
    {
      path: '/error/404',
      name: 'NotFoundPage',
      component: () => import('../components/error/NotFoundPage.vue')
    },
    {
      path: '/error/403',
      name: 'NoPermissionPage',
      component: () => import('../components/error/NoPermissionPage.vue')
    },
    {
      path: '/error/500',
      name: 'ErrorPage',
      component: () => import('../components/error/ErrorPage.vue')
    }
  ]
}
