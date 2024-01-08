import { useRouteMenuStore } from '@/stores/routeMenuStore'
import { useUserStore } from '@/stores/user'
import nprogress from '@/utils/nprogress'
import type { Router } from 'vue-router'

/**
 * 配置全局导航守卫
 * @param router 
 */
export function useRouterGuard(router: Router) {
  router.beforeEach(async (to, from, next) => {
    nprogress.start()

    /**
     * 白名单，直接跳转
     */
    if(['/login','/404'].includes(to.path)){
      next()
      return
    }
    const user=useUserStore()
    await user.refreshUserInfo()
    next()
  })

  router.afterEach(() => {
    nprogress.done()
   
  })
}
