import { createRouter, createWebHashHistory, createWebHistory } from 'vue-router'
import nprogress from '@/utils/nprogress'
import { getErrorRoutes } from '@/router/error'
import { getSystemRoute } from '@/router/sys'
import { getResultRoutes } from '@/router/result'
import { useRouteMenuStore } from '@/stores/routeMenuStore'
import { getExampleRoutes } from '@/router/example'

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'BasicLayout',
      component: () => import('../layout/BasicLayout.vue'),
      children: [
        ...getResultRoutes(),
        ...getSystemRoute(),
        ...getErrorRoutes(),
        ...getExampleRoutes(),
        {
          path: '/home',
          name: 'DefaultHomePage',
          component: () => import('../views/DefaultHomePage.vue')
        }
      ],
      redirect: '/home'
    },
    {
      path: '/login',
      name: '登录',
      component: () => import('../views/LoginView.vue')
    },
    {
      path: '/reload',
      name: '重新加载',
      component: () => import('../components/ReloadPage.vue')
    },
    {
      path: '/:pathMatch(.*)*',
      name: '404',
      redirect: '/error/404'
    }
  ]
})

router.beforeEach((to, from, next) => {
  nprogress.start()

  next()
})

router.afterEach(() => {
  nprogress.done()
  const routeMenuStore = useRouteMenuStore()
  routeMenuStore.refreshCurrentRouteInfo()
})

export default router
