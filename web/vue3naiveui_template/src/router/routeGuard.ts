import { useRouteMenuStore } from '@/stores/routeMenuStore'
import nprogress from '@/utils/nprogress'
import type { Router } from 'vue-router'

/**
 * 配置全局导航守卫
 * @param router 
 */
export function useRouterGuard(router: Router) {
  router.beforeEach((to, from, next) => {
    nprogress.start()
    next()
  })

  router.afterEach(() => {
    nprogress.done()
   
  })
}
