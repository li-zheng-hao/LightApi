<script setup lang="ts">

import type {RouteItem} from "@/stores/routeMenuStore";
import SvgIcon from "@/components/icons/SvgIcon.vue";

const props = defineProps<{
  routeInfos:RouteItem[]
}>()
</script>

<template>
  <div>
    <template v-for="item in props.routeInfos" :key="item.path">
      <!--      分为两种方式渲染：有子菜单和没有子菜单-->
      <el-sub-menu
          :index="item.key"
          v-if="item.children?.length > 0"
      >
        <template #title>
            <svg-icon :name="item.icon" class="mr-2"></svg-icon>
            <span>{{ item.label }}</span>
        </template>
        <!--        有子菜单的继续遍历（递归）-->
        <MenuTree :route-infos="item.children"></MenuTree>
      </el-sub-menu>
      <!--      没有子菜单-->
      <el-menu-item :index="item.routePath" v-if="!item.children || item.children.length == 0">
        <span>{{ item.label }}</span>
      </el-menu-item>
    </template>
  </div>
</template>

<style scoped>

</style>