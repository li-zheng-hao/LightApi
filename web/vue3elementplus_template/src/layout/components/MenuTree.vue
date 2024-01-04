<script setup lang="ts">
import type { RouteItem } from '@/stores/routeMenuStore'
import SvgIcon from '@/components/icons/SvgIcon.vue'

const props = defineProps<{
  routeInfos: RouteItem[]
}>()
</script>

<template>
    <template v-for="item in props.routeInfos" :key="item.path">
      <!--      分为两种方式渲染：有子菜单和没有子菜单-->
      <el-sub-menu :index="item.key" v-if="item.children?.length > 0">
        <template #title>
          <el-icon>
            <svg-icon :name="item.icon" :size="20"></svg-icon>
          </el-icon>
            <span class="pl-2">{{ item.label }}</span>
        </template>
        
        <!--        有子菜单的继续遍历（递归）-->
        <MenuTree :route-infos="item.children"></MenuTree>
      </el-sub-menu>
      <!--      没有子菜单-->
      <el-menu-item :index="item.routePath" v-if="!item.children || item.children.length == 0">
        <el-icon v-if="item.icon">
        <svg-icon :name="item.icon"  :size="20"></svg-icon>
      </el-icon>
        <template #title>
          <span class="pl-2">{{ item.label }}</span></template
        >
      </el-menu-item>
    </template>
</template>

<style scoped></style>
