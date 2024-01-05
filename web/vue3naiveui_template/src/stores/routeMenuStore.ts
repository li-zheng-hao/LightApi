import { defineStore } from 'pinia'
import router, { GlobalRoutes } from '@/router'
import { getAllMenus } from '@/config/menu'
import { _ } from '@/utils/common'
import { findRoute } from '@/utils/route-util'

export const useRouteMenuStore = defineStore('routeMenu', {
  state: () => ({
    routeMenus: [] as RouteItem[],
    currentRouteInfo: null as CurRouteItem,
    openedRouteInfo: [] as RouteItem[],
    menuCollapsed: false,
    // 需要缓存的组件 name 列表
    includedKeepAliveComponentsList: []
  }),
  // 相当于 computed
  getters: {},
  actions: {
    getAndUpdateAllRouteInfo(refresh: boolean = false): RouteItem[] {
      if (this.routeMenus.length > 0 && !refresh) return this.routeMenus
      this.routeMenus = getAllMenus()
      return this.routeMenus
    },
    findRouteItem(key: string): RouteItem | null {
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
    refreshCurrentRouteInfo(): CurRouteItem {
      let res = <CurRouteItem>{ paths: [], routePath: '' }
      let cur = router.currentRoute
      if (!cur) return cur
      this.routeMenus.some((item) => {
        if (item.routePath == cur.value.path) {
          res = <CurRouteItem>{
            paths: [item.label],
            routePath: item.routePath
          }
          return true
        }
        if (item.children) {
          item.children.some((child) => {
            if (child.routePath == cur.value.path) {
              res = <CurRouteItem>{
                paths: [item.label, child.label],
                routePath: child.routePath
              }
              return true
            }
            if (child.children) {
              child.children.some((grandChild) => {
                if (grandChild.routePath == cur.value.path) {
                  res = <CurRouteItem>{
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
      this.currentRouteInfo = res
      return res
    },
    removeOpenRoute(routePath: string | null | undefined): boolean {
      if (!routePath) return false
      if (this.openedRouteInfo?.length == 1) {
        window.$message.info('当前已经是最后一个页面了')
        return false
      }
      const index = _.findIndex(this.openedRouteInfo, (item) => item.routePath == routePath)
      if (index != -1) {
        this.openedRouteInfo.splice(index, 1)
      }
      if (this.openedRouteInfo.length == 0) {
        router.push('/')
        this.openedRouteInfo.push(this.findRouteItem('/'))
      } else {
        router.push(this.openedRouteInfo[this.openedRouteInfo.length - 1].routePath)
      }
      const route = findRoute(routePath, GlobalRoutes)
      if (route) this.removeIncludedKeepAliveComponents(route.name)
      return true
    },
    addOpenRoute(key: string) {
      let item = this.findRouteItem(key)
      if (item) {
        let index = _.findIndex(this.openedRouteInfo, (item) => item.routePath == key)
        if (index == -1) {
          this.openedRouteInfo.push(item)
        }
      }
      const route = findRoute(key, GlobalRoutes)
      console.log(key, route)
      if (route) this.addIncludedKeepAliveComponents(route.name)
    },
    removeOtherOpenRoute(routePath: string | null | undefined) {
      if (!routePath) return
      let item = this.findRouteItem(routePath)
      if (item) {
        this.openedRouteInfo = [item]
      }
      const route = findRoute(routePath, GlobalRoutes)

      if (route) {
        this.clearIncludedKeepAliveComponents()
        this.addIncludedKeepAliveComponents(route.name)
      }
    },
    removeAllOpenRoute() {
      if (this.openedRouteInfo.length == 0) return
      if (this.openedRouteInfo.length == 1 && this.openedRouteInfo[0].routePath == '/home') return
      this.openedRouteInfo.length = 0
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
  }
})

export interface RouteItem {
  label: string
  key: string
  icon: string | null
  routePath: string | null | undefined
  children: RouteItem[] | null
}

export interface CurRouteItem {
  paths: string[]
  routePath: string
}
