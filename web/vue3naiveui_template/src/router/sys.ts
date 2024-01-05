export function getSystemRoute() {
  return [
    {
      path: '/sys/menu',
      name: '菜单权限管理',
      component: () => import('../views/SystemMenuRolePage.vue')
    },
    {
      path: '/sys/role',
      name: '角色权限管理',
      component: () => import('../views/SystemUserRolePage.vue')
    }
  ]
}
