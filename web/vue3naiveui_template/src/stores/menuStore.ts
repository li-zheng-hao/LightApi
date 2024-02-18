import { defineStore } from 'pinia'
import { getAllMenus } from '@/config/menu'
import router from "@/router";

export const useRouteMenuStore = defineStore('app-menu', {
  state: () => ({
    // 所有的菜单项
    routeMenus: [] as MenuItem[],
    // 当前面包屑
    currentBreadcrumbInfo: {} as CurMenuItem,
    menuCollapsed: false
  }),
  // 相当于 computed
  getters: {},
  actions: {
    refreshAllMenuInfo(refresh: boolean = false): MenuItem[] {
      if (this.routeMenus.length > 0 && !refresh) return this.routeMenus
      this.routeMenus = getAllMenus()
      return this.routeMenus
    },
    /**
     * 查找菜单选项
     * @param key 
     * @param items 
     * @returns 
     */
    findMenuItem(key: string, items: MenuItem[]|undefined ): MenuItem | null {
      if(!items)
        items=this.routeMenus;
      for (const item of items) {
        if (item.key === key) {
          return item;
        }
        if (item.children) {
          const found = this.findMenuItem(key, item.children);
          if (found) {
            return found;
          }
        }
      }
      return null;
    },
    refreshCurrentBreadcrumbInfo(): CurMenuItem {
      const  findMenuItemByPath=(path: string, items: MenuItem[], paths: string[] ): CurMenuItem | null =>{
        if(!items)
          items= this.routeMenus;
        for (const item of items) {
          if (item.routePath === path) {
            return { paths: [...paths, item.label], routePath: item.routePath };
          }
          if (item.children) {
            const found = findMenuItemByPath(path, item.children, [...paths, item.label]);
            if (found) {
              return found;
            }
          }
        }
        return null;
      }
      const cur = router.currentRoute;
      if (!cur) return cur;
      const res = findMenuItemByPath(cur.value.path,this.routeMenus,[]);
      this.currentBreadcrumbInfo = res || { paths: [], routePath: '' };
      return this.currentBreadcrumbInfo;
    },
    setCurrentBreadcrumbInfo(paths:string[]){
      this.currentBreadcrumbInfo =  { paths: paths, routePath: '' };
      return this.currentBreadcrumbInfo;
    }
    
  },
  persist: {
    paths: ['currentBreadcrumbInfo'],
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
