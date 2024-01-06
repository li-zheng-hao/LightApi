<script setup lang="ts">
import LeftMenu from '@/layout/components/LeftMenu.vue'
import TopHeader from '@/layout/components/TopHeader.vue'
import TagsBar from '@/layout/components/TagsBar.vue'

import { useRouteMenuStore } from '@/stores/routeMenuStore'
import { onMounted, onUnmounted, ref, watch } from 'vue'
import { EventBus, EventBusEvents } from '@/utils/eventbus'
import { useRoute } from 'vue-router'

let routeMenuStore = useRouteMenuStore()
const route = useRoute()
const data = ref({
  showComponent: true
})
// 初始化菜单面包屑信息
routeMenuStore.refreshAllMenuInfo()
routeMenuStore.refreshCurrentBreadcrumbInfo()

// 监听路由变化，更新当前面包屑信息和标签页
watch(
  () => route.fullPath,
  (newValue) => {
    console.log('route.fullPath', newValue)
    routeMenuStore.addOpenTab(newValue)
    routeMenuStore.refreshCurrentBreadcrumbInfo()
  }
)
const reloadPage = () => {
  const compName = route.name as string
  routeMenuStore.removeIncludedKeepAliveComponents(compName)
  data.value.showComponent = false

  // 模拟出刷新页面重新加载的效果
  setTimeout(() => {
    routeMenuStore.addIncludedKeepAliveComponents(compName)
    data.value.showComponent = true
  }, 250)
}
onMounted(() => {
  EventBus.on(EventBusEvents.RELOAD_PAGE, reloadPage)
})
onUnmounted(() => {
  EventBus.off(EventBusEvents.RELOAD_PAGE, reloadPage)
})
</script>

<template>
  <n-layout has-sider class="w-full h-full bg-#f5f7f9">
    <n-layout-sider
      :inverted="true"
      bordered
      collapse-mode="width"
      :collapsed-width="64"
      :width="200"
      show-trigger="bar"
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
      <n-layout-header>
        <top-header />
      </n-layout-header>
      <tags-bar class="p-2 pb-0"></tags-bar>
      <n-layout-content content-style="box-border" class="h-full box-border bg-#f5f7f9 p-2">
        <n-scrollbar>
          <router-view v-slot="{ Component }">
            <keep-alive :max="20" :include="routeMenuStore.includedKeepAliveComponentsList">
              <component :is="Component" :key="route.name" v-if="data.showComponent" />
            </keep-alive>
            <div
              v-if="!data.showComponent"
              class="w-full h-80vh p-10 flex flex-items-center justify-center"
            >
              <n-spin size="large">
                <template #description> 加载中... </template>
              </n-spin>
            </div>
          </router-view>
        </n-scrollbar>
      </n-layout-content>
    </n-layout>
  </n-layout>
</template>

<style scoped></style>
