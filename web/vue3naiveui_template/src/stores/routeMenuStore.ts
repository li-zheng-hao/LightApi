import { defineStore } from 'pinia'
import router from '@/router'
import { getAllMenus } from '@/config/menu'
import _ from '@/utils/common'

export const useRouteMenuStore = defineStore('routeMenu', {
  state: () => ({
    routeMenus: [] as RouteItem[],
    currentRouteInfo: null as CurRouteItem | null,
    openedRouteInfo: [] as RouteItem[],
    menuCollapsed: false,
    // 不需要缓存的组件 name 列表
    excludedKeepAliveComponentsList: []
  }),
  getters: {},
  actions: {
    getAllRouteInfo(refresh: boolean = false): RouteItem[] {
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
    },
    removeOtherOpenRoute(key: string | null | undefined) {
      if (!key) return
      let item = this.findRouteItem(key)
      if (item) {
        this.openedRouteInfo = [item]
      }
    },
    removeAllOpenRoute() {
      if (this.openedRouteInfo.length == 0) return
      if (this.openedRouteInfo.length == 1 && this.openedRouteInfo[0].routePath == '/') return
      this.openedRouteInfo = []
      router.push('/')
      this.openedRouteInfo.push(this.findRouteItem('/'))
    },
    /**
     * 添加不需要缓存的组件
     * @param name 组件名称
     */
    addExcludedKeepAliveComponents(name: string) {
      if (this.excludedKeepAliveComponentsList.indexOf(name) == -1) {
        this.excludedKeepAliveComponentsList.push(name)
      }
    },
    /**
     * 移除不需要缓存的组件
     * @param name 组件名称
     */
    removeExcludedKeepAliveComponents(name: string) {
      let index = this.excludedKeepAliveComponentsList.indexOf(name)
      if (index != -1) {
        this.excludedKeepAliveComponentsList.splice(index, 1)
      }
    },
    /**
     * 清空不需要缓存的组件列表
     */
    clearExcludedKeepAliveComponents() {
      this.excludedKeepAliveComponentsList.length = 0
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
