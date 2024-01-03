<script setup lang="ts">
import {ref} from 'vue'
import LeftMenu from "@/layout/components/LeftMenu.vue";
import TopHeader from "@/layout/components/TopHeader.vue";
import TagsBar from "@/layout/components/TagsBar.vue";
import TabPage from "@/components/TabPage.vue";
import DefaultHomePage from "@/views/DefaultHomePage.vue";
import router from "@/router";

import {useRouteMenuStore} from "@/stores/routeMenuStore";

let routeMenuStore = useRouteMenuStore()
</script>

<template>
  <el-container class="w-full h-full bg-#f5f7f9">
    <el-aside width="200px" class="h-full bg-white" >    <LeftMenu />
    </el-aside>
    <el-container>
      <el-header class="top-header-box"><top-header/></el-header>
      <el-main>
        <tags-bar class="p-2 pb-0"></tags-bar>
          <DefaultHomePage v-if="router.currentRoute.value.path=='/'" class="w-full h-full"/>
          <router-view class="w-full h-full" v-slot="{ Component }">
            <keep-alive>
              <component :is="Component" />
            </keep-alive>
          </router-view>
      </el-main>
    </el-container>
  </el-container>
</template>

<style scoped lang="less">

.top-header-box{
  padding: 0;
  background-color: #FFFFFF;
}
.el-aside{
  border-right: 1px #e7e7e7 solid;
}
</style>
