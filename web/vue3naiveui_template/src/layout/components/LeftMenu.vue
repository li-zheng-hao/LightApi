<script lang="tsx" setup>
import { defineComponent, h, ref, type Component, onMounted } from 'vue'
import { NIcon } from 'naive-ui'
import type { MenuOption } from 'naive-ui'
import { RouteItem, useRouteMenuStore } from '@/stores/routeMenuStore'
import SvgIcon from '@/components/icons/SvgIcon.vue'
import { RouterLink } from 'vue-router'
import _, { divide } from 'lodash'
import router from '@/router'
import Menu from '@/mock/menu'
function renderIcon(icon: string | null | undefined) {
  if (!icon) return null
  return () => h(SvgIcon, { name: icon })
}
const routeMenuStore = useRouteMenuStore()

const data = ref<any>({
  collapsed: false,
  menuOptions: [] as MenuOption[],
  expandKeys: ['/sys/menu']
})

const handleUpdateRoute = (key: string) => {
  routeMenuStore.addOpenRoute(key)
}
function addMenuOptions(routeInfo: RouteItem[], menuOptions: MenuOption[]) {
  routeInfo.forEach((value, key) => {
    const menuOption = {
      label:
        value.children?.length ?? 0 > 0
          ? () => <div>{value.label}</div>
          : () => <RouterLink to={value.routePath ?? ''}>{value.label}</RouterLink>,
      key: value.key,
      icon: () =>
        value.icon ? (
          <n-icon>
            <svg-icon-raw name={value.icon}></svg-icon-raw>{' '}
          </n-icon>
        ) : null,
      children: null as any
    }

    if (!_.isEmpty(value.children)) {
      menuOption.children = []
      addMenuOptions(value.children, menuOption.children)
    }

    menuOptions.push(menuOption)
  })
}

onMounted(() => {
  const routeInfo = routeMenuStore.getAllRouteInfo()
  const menuOptions = [] as MenuOption[]

  addMenuOptions(routeInfo, menuOptions)

  data.value.menuOptions = menuOptions
  routeMenuStore.addOpenRoute(router.currentRoute.value.path)
})

const props = defineProps(['collapsed'])
</script>

<template>
  <n-menu
    :inverted="true"
    v-model:value="routeMenuStore.currentRouteInfo!.routePath"
    :collapsed="routeMenuStore.menuCollapsed"
    :collapsed-width="64"
    :options="data.menuOptions"
    :accordion="true"
    :indent="20"
    :watch-props="['defaultExpandedKeys']"
    @update:value="handleUpdateRoute"
  />
</template>

<style scoped></style>
