import { defineStore } from 'pinia'
import router, { GlobalRoutes } from '@/router'
import { getAllMenus } from '@/config/menu'
import { _ } from '@/utils/common'
import { findRoute } from '../utils/routeUtil'

export const useRouteMenuStore = defineStore('app-menu', {
  state: () => ({
    // 所有的菜单项
    routeMenus: [] as MenuItem[],
    // 当前面包屑
    currentBreadcrumbInfo: null as CurMenuItem,
    openedTabs: [] as MenuItem[],
    menuCollapsed: false,
    // 需要缓存的组件 name 列表
    includedKeepAliveComponentsList: []
  }),
  // 相当于 computed
  getters: {},
  actions: {
    refreshAllMenuInfo(refresh: boolean = false): MenuItem[] {
      if (this.routeMenus.length > 0 && !refresh) return this.routeMenus
      this.routeMenus = getAllMenus()
      return this.routeMenus
    },
    findMenuItem(key: string): MenuItem | null {
      let res = null
      this.routeMenus.some((item) => {
        if (item.key == key) {
          res = item
          return true
        }
        if (item.children) {
          item.children.some((child) => {
            if (child.key == key) {
              res = child
              return true
            }
            if (child.children) {
              child.children.some((grandChild) => {
                if (grandChild.key == key) {
                  res = grandChild
                  return true
                }
                return false
              })
            }
            return false
          })
        }
        return false
      })
      return res
    },
    refreshCurrentBreadcrumbInfo(): CurMenuItem {
      let res = <CurMenuItem>{ paths: [], routePath: '' }
      let cur = router.currentRoute
      if (!cur) return cur
      this.routeMenus.some((item) => {
        if (item.routePath == cur.value.path) {
          res = <CurMenuItem>{
            paths: [item.label],
            routePath: item.routePath
          }
          return true
        }
        if (item.children) {
          item.children.some((child) => {
            if (child.routePath == cur.value.path) {
              res = <CurMenuItem>{
                paths: [item.label, child.label],
                routePath: child.routePath
              }

              return true
            }
            if (child.children) {
              child.children.some((grandChild) => {
                if (grandChild.routePath == cur.value.path) {
                  res = <CurMenuItem>{
                    paths: [item.label, child.label, grandChild.label],
                    routePath: grandChild.routePath
                  }

                  return true
                }

                return false
              })
            }
            return false
          })
        }
        return false
      })
      this.currentBreadcrumbInfo = res
      return res
    },
    removeOpenTab(routePath: string | null | undefined): boolean {
      if (!routePath) return false
      if (this.openedTabs?.length == 1) {
        window.$message.info('当前已经是最后一个页面了')
        return false
      }
      const index = _.findIndex(this.openedTabs, (item) => item.routePath == routePath)
      if (index != -1) {
        this.openedTabs.splice(index, 1)
      }
      if (this.openedTabs.length == 0) {
        router.push('/home')
        this.openedTabs.push(this.findMenuItem('/home'))
      } else {
        router.push(this.openedTabs[this.openedTabs.length - 1].routePath)
      }
      const route = findRoute(routePath, GlobalRoutes)
      if (route) this.removeIncludedKeepAliveComponents(route.name)
      return true
    },
    addOpenTab(key: string) {
      const item = this.findMenuItem(key)
      if (item) {
        const index = _.findIndex(this.openedTabs, (item) => item.routePath == key)
        if (index == -1) {
          this.openedTabs.push(item)
        }
      }
      const route = findRoute(key, GlobalRoutes)
      if (route) this.addIncludedKeepAliveComponents(route.name)
    },
    removeOtherOpenTabs(routePath: string | null | undefined) {
      if (!routePath) return
      const item = this.findMenuItem(routePath)
      if (item) {
        this.openedTabs = [item]
      }
      const route = findRoute(routePath, GlobalRoutes)

      if (route) {
        this.clearIncludedKeepAliveComponents()
        this.addIncludedKeepAliveComponents(route.name)
      }
    },
    removeAllOpenTabs() {
      if (this.openedTabs.length == 0) return
      if (this.openedTabs.length == 1 && this.openedTabs[0].routePath == '/home') return
      this.openedTabs.length = 0
      this.clearIncludedKeepAliveComponents()
    },
    /**
     * 添加需要缓存的组件
     * @param name 组件名称
     */
    addIncludedKeepAliveComponents(name: string) {
      if (this.includedKeepAliveComponentsList.indexOf(name) == -1) {
        this.includedKeepAliveComponentsList.push(name)
      }
    },
    /**
     * 移除需要缓存的组件
     * @param name 组件名称
     */
    removeIncludedKeepAliveComponents(name: string) {
      let index = this.includedKeepAliveComponentsList.indexOf(name)
      if (index != -1) {
        this.includedKeepAliveComponentsList.splice(index, 1)
      }
    },
    /**
     * 清空需要缓存的组件列表
     */
    clearIncludedKeepAliveComponents() {
      this.includedKeepAliveComponentsList.length = 0
    }
  },
  persist: {
    paths: ['currentBreadcrumbInfo','openedTabs','includedKeepAliveComponentsList'],
  }

})

export interface MenuItem {
  label: string
  key: string
  icon: string | null
  routePath: string | null | undefined
  children: MenuItem[] | null
}

export interface CurMenuItem {
  paths: string[]
  routePath: string
}
