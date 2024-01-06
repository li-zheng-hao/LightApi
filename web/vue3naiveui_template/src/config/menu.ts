// 全局的菜单配置 (根据当前登录用户权限生成左侧菜单栏)

import type { MenuItem } from "@/stores/routeMenuStore";

// 也可以从后端获取
export function getAllMenus() {
  return [
    {
      label: '首页',
      key: '/home',
      icon: 'DashboardOutlined',
      routePath: '/home'
    },
    {
      label: '系统设置',
      key: '/sys',
      icon: 'SettingOutlined',
      routePath: null,
      children: [
        {
          label: '角色权限管理',
          key: '/sys/role',
          routePath: '/sys/role'
        },
        {
          label: '菜单权限管理',
          key: '/sys/menu',
          routePath: '/sys/menu'
        }
      ]
    },
    {
      label: '异常页面',
      key: '/error',
      icon: 'WarningOutlined',
      routePath: null,
      children: [
        {
          label: '404',
          key: '/error/404',
          routePath: '/error/404'
        },
        {
          label: '403',
          key: '/error/403',
          routePath: '/error/403'
        },
        {
          label: '500',
          key: '/error/500',
          routePath: '/error/500'
        }
      ]
    },
    {
      label: '结果页面',
      key: '/result',
      icon: 'UserOutlined',
      routePath: null,
      children: [
        {
          label: '成功',
          key: '/result/success',
          routePath: '/result/success'
        },
        {
          label: '失败',
          key: '/result/fail',
          routePath: '/result/fail'
        },
        {
          label: '信息',
          key: '/result/info',
          routePath: '/result/info'
        }
      ]
    },
    {
      label: '组件示例',
      key: '/example',
      icon: 'UserOutlined',
      routePath: null,
      children: [
        {
          label: '按钮',
          key: '/example/button',
          routePath: null,
          icon: 'HandClick',
          children: [
            {
              label: '按钮1',
              key: '/example/button/button1',
              routePath: '/example/button/button1'
            },
            {
              label: '按钮2',
              key: '/example/button/button2',
              routePath: '/example/button/button2'
            }
          ]
        },
        {
          label: '表格示例',
          key: '/example/table',
          routePath: '/example/table',
          icon: 'TableIcon'
        }
      ]
    },
    {
      label: '第三方内嵌页面',
      key: '/thirdpart',
      icon: 'Share',
      routePath: '/thirdpart'
    }
  ] as MenuItem[]
}
