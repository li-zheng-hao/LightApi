import { createRouter, createWebHashHistory, createWebHistory } from 'vue-router'
import nprogress from '@/utils/nprogress'
import { getErrorRoutes } from './routeConfig/error'
import { getSystemRoute } from './routeConfig/sys'
import { getResultRoutes } from './routeConfig/result'
import { useRouteMenuStore } from '@/stores/menuStore'
import { getExampleRoutes } from './routeConfig/example'
import { useRouterGuard } from './routeGuard'

export const GlobalRoutes = [
  {
    path: '/',
    name: 'BasicLayout',
    component: () => import('@/layout/BasicLayout.vue'),
    children: [
      ...getResultRoutes(),
      ...getSystemRoute(),
      ...getErrorRoutes(),
      ...getExampleRoutes(),
      {
        path: '/home',
        name: 'DefaultHomePage',
        component: () => import('@/views/DefaultHomePage.vue')
      },
      {
        path: '/thirdpart',
        name: 'ThirdPartPage',
        component: () => import('@/views/ThirdPartPage.vue')
      },{
        path: '/usersetting',
        name: 'UserSettingView',
        component: () => import('@/views/UserSettingView.vue')
      }
    ],
    redirect: '/home'
  },
  {
    path: '/login',
    name: 'LoginView',
    component: () => import('@/views/LoginView.vue')
  },
  {
    path: '/single',
    name: 'NewSinglePage',
    component: () => import('@/views/NewSinglePage.vue')
  },
  {
    path: '/:pathMatch(.*)*',
    name: '404',
    redirect: '/error/404'
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: GlobalRoutes
})

useRouterGuard(router)

export default router
