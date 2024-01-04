<script lang="tsx" setup>
import { defineComponent, h, ref, type Component, onMounted } from 'vue'
import { NIcon } from 'naive-ui'
import type { MenuOption } from 'naive-ui'
import { useRouteMenuStore } from '@/stores/routeMenuStore'
import SvgIcon from '@/components/icons/SvgIcon.vue'
import { RouterLink, useRoute } from 'vue-router'
import MenuTree from '@/layout/components/MenuTree.vue'

const routeMenuStore = useRouteMenuStore()

const route = useRoute()

let routeInfo = routeMenuStore.getAllRouteInfo()

onMounted(() => {})

const props = defineProps(['collapsed'])

const handleSelect = (key: any, keyPath: any) => {
  routeMenuStore.addOpenRoute(key)
}
</script>

<template>
  <el-scrollbar>
    <div
      class="system-icon-box flex w-full h-20 flex-items-center flex-justify-center color-white gap-1"
    >
      <div class="font-2 font-size-4 bg-#2d8cf0 p-1 pt-2 pb-2 rd-2px">LZH</div>
      <div v-if="!routeMenuStore.menuCollapsed">Vue3模板</div>
    </div>
    <el-menu
      :default-active="route.path"
      mode="vertical"
      :collapse-transition="false"
      router
      unique-opened
      :collapse="routeMenuStore.menuCollapsed"
      @select="handleSelect"
      
    >
      <MenuTree :route-infos="routeInfo"></MenuTree>
    </el-menu>
  </el-scrollbar>
</template>

<style scoped lang="less">
.el-menu {
  border-right: none;
  --el-menu-bg-color: #001428;
}
:deep(.el-menu-item) {
  --el-menu-text-color: #fff;
  --el-menu-hover-bg-color: var(--el-color-primary);
}
:deep(.el-menu-item:hover) {
    --el-menu-hover-bg-color: var(--el-color-primary);
    background-color: var(--el-color-primary);
  }
:deep(.el-sub-menu__title) {
  --el-menu-text-color: #fff;
  --el-menu-hover-bg-color: var(--el-color-primary);
}

</style>
