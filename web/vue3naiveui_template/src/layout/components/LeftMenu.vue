<script lang="tsx" setup>
import { ref, onMounted, watch } from 'vue'
import type { MenuOption } from 'naive-ui'
import { type MenuItem, useRouteMenuStore } from '@/stores/menuStore'
import { RouterLink } from 'vue-router'
import { _ } from '@/utils/common'

const routeMenuStore = useRouteMenuStore()

const data = ref<any>({
  collapsed: false,
  menuOptions: [] as MenuOption[]
})

/**
 * 生成菜单项的label
 * @param value 
 */
function generateLabel(value:MenuItem) {
  if(value.children?.length ?? 0 > 0) {
    return () => <div> {value.label}</div>
  } else if(value.routePath?.startsWith("http")) 
  {
    return () => <a href={value.routePath!} target="_blank">{value.label}</a>
  }else
  {
    return () => <RouterLink to={value.routePath ?? '' } >{value.label}</RouterLink>
  }
 
}

/**
 * 递归添加菜单项
 * @param routeInfo 
 * @param menuOptions 
 */
function addMenuOptions(routeInfo: MenuItem[] | null, menuOptions: MenuOption[]) {
  if (!routeInfo) return
  routeInfo.forEach((value, key) => {
    const menuOption = {
      label:
       generateLabel(value),
      key: value.key,
      icon: () =>
        value.icon ? (
          <n-icon>
            <svg-icon-raw name={value.icon}></svg-icon-raw>
          </n-icon>
        ) : null,
      children: undefined as MenuOption[] | undefined
    }

    if (!_.isEmpty(value.children)) {
      menuOption.children = []
      addMenuOptions(value.children, menuOption.children)
    }

    menuOptions.push(menuOption)
  })
}

onMounted(() => {
  const menuItems = routeMenuStore.refreshAllMenuInfo()
  const menuOptions = [] as MenuOption[]

  addMenuOptions(menuItems, menuOptions)

  data.value.menuOptions = menuOptions
})
</script>

<template>
  <n-menu
    :inverted="true"
    v-model:value="routeMenuStore.currentBreadcrumbInfo!.routePath"
    :collapsed="routeMenuStore.menuCollapsed"
    :collapsed-width="64"
    :options="data.menuOptions"
    :accordion="true"
    :indent="20"
    :watch-props="['defaultExpandedKeys']"
  />
</template>

<style scoped></style>
