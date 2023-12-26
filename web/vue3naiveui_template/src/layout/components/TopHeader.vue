<script setup lang="ts">

import SvgIcon from "@/components/icons/SvgIcon.vue";
import {computed, onMounted, ref} from "vue";
import {useRouteMenuStore} from "@/stores/routeMenuStore";
import router from "@/router";
const routeMenuStore=useRouteMenuStore()


onMounted(() => {
  routeMenuStore.refreshCurrentRouteInfo()
})
</script>

<template>
  <div class="layout-header">
    <div class="layout-header-left flex gap-4 pl-4 pr-4 flex-items-center">
      <svg-icon name="MenuFoldOutlined" class="cursor-pointer" v-if="!routeMenuStore.menuCollapsed" @click="routeMenuStore.menuCollapsed=!routeMenuStore.menuCollapsed"></svg-icon>
      <svg-icon name="MenuUnfoldOutlined" class="cursor-pointer" v-if="routeMenuStore.menuCollapsed"  @click="routeMenuStore.menuCollapsed=!routeMenuStore.menuCollapsed"></svg-icon>
      <svg-icon name="ReloadOutlined" class="cursor-pointer" @click="router.push('/reload')"></svg-icon>
      <n-breadcrumb>
        <n-breadcrumb-item v-for="(value,key) in routeMenuStore.currentRouteInfo?.paths" >
          {{value}}
        </n-breadcrumb-item>
      </n-breadcrumb>
    </div>
    <div class="layout-header-right  flex gap-4 pl-4 pr-4 flex-items-center flex-shrink-0">
      <n-avatar
          :style="{
      color: 'yellow',
      backgroundColor: '#2d8cf0',
      cursor: 'pointer'
    }"
      >
        LZH
      </n-avatar>
      <div class="cursor-pointer">李正浩</div>
      <svg-icon name="SettingOutlined" class="cursor-pointer"></svg-icon>
    </div>
  </div>
</template>

<style scoped lang="less">
.layout-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0;
  height: 64px;
  box-shadow: 0 1px 4px rgb(0 21 41 / 8%);
  transition: all 0.2s ease-in-out;
  width: 100%;
  z-index: -1;
  box-sizing: border-box;
}

.layout-header-left {

}
</style>
