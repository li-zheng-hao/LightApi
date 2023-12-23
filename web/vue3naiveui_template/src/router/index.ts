import { createRouter, createWebHistory } from 'vue-router'
import nprogress from "@/utils/nprogress";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('../views/HomeView.vue')
    }
  ]
})

router.beforeEach((to, from, next) => {
  nprogress.start()
  next()
})

router.afterEach(() => {
  nprogress.done()
})

export default router
