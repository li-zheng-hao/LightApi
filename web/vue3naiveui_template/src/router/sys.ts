export function getSystemRoute() {
  return [
    {
      path: '/sys/menu',
      name: 'SystemMenuRolePage',
      component: () => import('../views/SystemMenuRolePage.vue')
    },
    {
      path: '/sys/role',
      name: 'SystemUserRolePage',
      component: () => import('../views/SystemUserRolePage.vue')
    }
  ]
}
