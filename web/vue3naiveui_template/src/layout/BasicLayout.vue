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
      <div class=" flex-items-center justify-center flex   h-20 gap-2">
        <n-avatar
            :style="{
      color: 'yellow',
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
        <top-header/>
      </n-layout-header>
      <tags-bar class="p-2 pb-0"></tags-bar>
      <n-layout-content content-style="box-border" class="h-full box-border bg-#f5f7f9 p-2">
        <DefaultHomePage v-if="router.currentRoute.value.path=='/'" class="w-full h-full"/>
        <router-view class="w-full h-full" v-slot="{ Component }">
          <keep-alive>
            <component :is="Component" />
          </keep-alive>
        </router-view>
      </n-layout-content>
    </n-layout>
  </n-layout>
</template>

<style scoped>

</style>
