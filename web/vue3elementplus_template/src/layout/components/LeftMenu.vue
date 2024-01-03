<script lang="tsx" setup>
import {defineComponent, h, ref, type Component, onMounted} from 'vue'
import { NIcon } from 'naive-ui'
import type { MenuOption } from 'naive-ui'
import { useRouteMenuStore} from "@/stores/routeMenuStore";
import SvgIcon from "@/components/icons/SvgIcon.vue";
import {RouterLink, useRoute} from "vue-router";
import MenuTree from "@/layout/components/MenuTree.vue";

const routeMenuStore=useRouteMenuStore()

const route = useRoute()

let routeInfo=routeMenuStore.getAllRouteInfo()

onMounted(()=>{

})



const props=defineProps(['collapsed'])
</script>

<template>
  <el-scrollbar>
    <el-menu :default-active="route.path" mode="vertical"  router >
      <MenuTree :route-infos="routeInfo"></MenuTree>
    </el-menu>
  </el-scrollbar>
</template>

<style scoped>
.el-menu{
  border-right: none;
}
</style>
