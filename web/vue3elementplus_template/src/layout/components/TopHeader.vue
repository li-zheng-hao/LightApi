<script setup lang="ts">
import SvgIcon from '@/components/icons/SvgIcon.vue'
import { computed, onMounted, ref } from 'vue'
import { useRouteMenuStore } from '@/stores/routeMenuStore'
import router from '@/router'
const routeMenuStore = useRouteMenuStore()

const dropDownOptions = ref<any>([
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
onMounted(() => {
  routeMenuStore.refreshCurrentRouteInfo()
})
</script>

<template>
  <div class="layout-header">
    <div class="layout-header-left flex gap-4 pl-4 pr-4 flex-items-center">
      <el-tooltip >
        <template #content>
          {{routeMenuStore.menuCollapsed?"展开":"折叠"}}菜单
        </template>
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
      </el-tooltip>
     
         
        
      <el-tooltip content="刷新页面">
        <svg-icon name="ReloadOutlined" class="cursor-pointer"></svg-icon>
      </el-tooltip>

      <el-breadcrumb>
        <el-breadcrumb-item v-for="(value, key) in routeMenuStore.currentRouteInfo?.paths" :key="key">
          {{ value }}
        </el-breadcrumb-item>
      </el-breadcrumb>
    </div>
    <div class="layout-header-right flex gap-4 pl-4 pr-4 flex-items-center flex-shrink-0">

      <div>LZH</div>
      <div>李正浩</div>
      <el-dropdown class="pt-1px">
        <div><SvgIcon name="SettingOutlined" class="cursor-pointer"></SvgIcon></div>

        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item v-for="item in dropDownOptions" :key="item.key">
              {{item.label}}
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
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
  transition: all 0.2s ease-in-out;
  width: 100%;
  z-index: -1;
  box-sizing: border-box;
}
</style>
