<script setup lang="ts">
import SvgIcon from '@/components/icons/SvgIcon.vue'
import { onMounted, ref } from 'vue'
import { useRouteMenuStore } from '@/stores/routeMenuStore'
import router from '@/router'
import { NDropdown, type MenuOption } from 'naive-ui'
import { EventBus, EventBusEvents } from '@/utils/eventbus'
const routeMenuStore = useRouteMenuStore()

const dropDownOptions = ref<MenuOption[]>([
  {
    label: '个人中心',
    key: '个人中心'
  },
  {
    label: '个人设置',
    key: '个人设置'
  },
  {
    label: '退出登录',
    key: '退出登录'
  }
])

const handleSelect = (key: any, option: any) => {
  switch (key) {
    case '退出登录':
      router.push('/login')
      break
    default:
      break
  }
}

const reload = () => {
  EventBus.emit(EventBusEvents.RELOAD_PAGE)
}

</script>

<template>
  <div class="layout-header">
    <div class="layout-header-left flex gap-4 pl-4 pr-4 flex-items-center">
      <n-tooltip trigger="hover">
        <template #trigger>
          <svg-icon
            name="MenuFoldOutlined"
            class="cursor-pointer"
            v-if="!routeMenuStore.menuCollapsed"
            @click="routeMenuStore.menuCollapsed = !routeMenuStore.menuCollapsed"
          ></svg-icon>
          <svg-icon
            name="MenuUnfoldOutlined"
            class="cursor-pointer"
            v-if="routeMenuStore.menuCollapsed"
            @click="routeMenuStore.menuCollapsed = !routeMenuStore.menuCollapsed"
          ></svg-icon>
        </template>
        {{ routeMenuStore.menuCollapsed ? '展开' : '折叠' }}菜单
      </n-tooltip>

      <n-tooltip trigger="hover">
        <template #trigger>
          <svg-icon name="ReloadOutlined" class="cursor-pointer" @click="reload"></svg-icon>
        </template>
        刷新页面
      </n-tooltip>

      <n-breadcrumb>
        <n-breadcrumb-item
          v-for="(value, key) in routeMenuStore.currentRouteInfo?.paths"
          :key="key"
        >
          {{ value }}
        </n-breadcrumb-item>
      </n-breadcrumb>
    </div>
    <div class="layout-header-right flex gap-4 pl-4 pr-4 flex-items-center flex-shrink-0">
      <n-avatar
        :style="{
          color: 'white',
          backgroundColor: '#2d8cf0'
        }"
      >
        LZH
      </n-avatar>
      <div>李正浩</div>
      <n-dropdown
        placement="bottom-start"
        trigger="click"
        :options="dropDownOptions"
        @select="handleSelect"
      >
        <SvgIcon name="SettingOutlined" class="cursor-pointer"></SvgIcon>
      </n-dropdown>
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
</style>
