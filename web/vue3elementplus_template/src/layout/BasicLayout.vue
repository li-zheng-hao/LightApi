<script setup lang="ts">
import { nextTick, onMounted, ref, unref } from 'vue'
import LeftMenu from '@/layout/components/LeftMenu.vue'
import TopHeader from '@/layout/components/TopHeader.vue'
import TagsBar from '@/layout/components/TagsBar.vue'
import { useRouteMenuStore } from '@/stores/routeMenuStore'
import { useRoute, type RouteLocationNormalized } from 'vue-router'
import { EventBus, EventBusEvents } from '@/utils/eventbus'

let routeMenuStore = useRouteMenuStore()
const route = useRoute()
const data = ref({
  showComponent: true
})
const reloadPage = () => {
  console.log('刷新页面', route.name)
  routeMenuStore.addKeepAliveExcludeList(route.name as string)
  data.value.showComponent = false

  // 模拟出刷新页面重新加载的效果
  setTimeout(() => {
    routeMenuStore.clearKeepAliveExcludeList()
    data.value.showComponent = true
  }, 250)
}

onMounted(() => {
  routeMenuStore.addOpenRoute(route.fullPath as string)
  EventBus.on(EventBusEvents.RELOAD_PAGE, reloadPage)
})
</script>

<template>
  <div class="w-full h-full bg-#f5f7f9 root-box">
    <div :class="[routeMenuStore.menuCollapsed ? 'menu-collapsed' : '', 'left-box']">
      <LeftMenu />
    </div>
    <div class="w-full h-full flex flex-col">
      <div class="top-header-box"><top-header @reload-page="reloadPage" /></div>
      <tags-bar class="p-1"></tags-bar>
      <el-scrollbar v-if="data.showComponent">
        <router-view v-slot="{ Component }">
          <keep-alive :exclude="routeMenuStore.keepAliveExcludeList">
            <component
              :is="Component"
              :key="route.name"
              v-if="data.showComponent"
              class="m-1 min-h-100"
            />
          </keep-alive>
        </router-view>
      </el-scrollbar>

      <div v-if="!data.showComponent" class="w-full h-full p-10" v-loading="!data.showComponent" element-loading-text="加载中..." element-loading-background="rgba(245, 247, 249, 0.8)">
       
      </div>
    </div>
  </div>
</template>

<style scoped lang="less">
.top-header-box {
  padding: 0;
  background-color: #ffffff;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.18);
}
.left-box {
  border-right: 1px #e7e7e7 solid;
  background-color: #001428;
  width: 200px;
  transition: width 0.1s ease-in-out;
}
.menu-collapsed {
  width: 64px;
}
.root-box {
  display: flex;
  height: 100%;
  width: 100%;
}
:deep(.el-loading-mask){
  margin: 10px;
}
</style>
