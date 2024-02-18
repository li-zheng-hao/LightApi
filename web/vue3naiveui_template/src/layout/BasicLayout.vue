<script setup lang="ts">
import LeftMenu from '@/layout/components/LeftMenu.vue'
import TopHeader from '@/layout/components/TopHeader.vue'
import { useRouteMenuStore } from '@/stores/menuStore'
import { watch } from 'vue'
import { useRoute } from 'vue-router'

let routeMenuStore = useRouteMenuStore()
const route = useRoute()
// 初始化菜单面包屑信息
routeMenuStore.refreshAllMenuInfo()
routeMenuStore.refreshCurrentBreadcrumbInfo()

// 监听路由变化，更新当前面包屑信息和标签页
watch(
  () => route.fullPath,
  () => {
    routeMenuStore.refreshCurrentBreadcrumbInfo()
  }
)
</script>

<template>
  <n-layout has-sider class="w-full h-full bg-#f5f7f9">
    <n-layout-sider
      :inverted="true"
      bordered
      collapse-mode="width"
      :collapsed-width="64"
      :width="200"
      v-model:collapsed="routeMenuStore.menuCollapsed"
      :native-scrollbar="false"
    >
      <div class="flex-items-center justify-center flex h-20 gap-2">
        <n-avatar
          :style="{
            color: 'white',
            backgroundColor: '#2d8cf0'
          }"
        >
          LZH
        </n-avatar>
        <div v-if="!routeMenuStore.menuCollapsed">Vue3模板</div>
      </div>
      <LeftMenu></LeftMenu>
    </n-layout-sider>
    <n-layout class="h-full box-border flex w-full bg-#f5f7f9" content-class="w-full flex flex-col">
      <n-layout-header class="bg-#f5f7f9">
        <top-header />
      </n-layout-header>
      <n-layout-content content-style="box-border" class="h-full box-border bg-#f5f7f9 p-2">
        <n-scrollbar>
          <router-view />
        </n-scrollbar>
      </n-layout-content>
    </n-layout>
  </n-layout>
</template>

<style scoped></style>
