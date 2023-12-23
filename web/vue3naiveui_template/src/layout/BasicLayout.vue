<script setup lang="ts">
import {ref} from 'vue'
import LeftMenu from "@/layout/LeftMenu.vue";
import TopHeader from "@/layout/TopHeader.vue";

let collapsed=ref(false)
</script>

<template>
  <n-layout has-sider class="w-full h-full">
    <n-layout-sider
        bordered
        collapse-mode="width"
        :collapsed-width="64"
        :width="240"
        show-trigger
        v-model:collapsed="collapsed"
        :native-scrollbar="false"
    >
      <LeftMenu ></LeftMenu>
    </n-layout-sider>
    <n-layout class="h-full box-border flex w-full"  content-class="w-full flex flex-col">
      <n-layout-header class="pt-2"><top-header/></n-layout-header>
      <n-layout-content content-style="box-border" class="h-full box-border">
        <div>中间区域</div>
      </n-layout-content>
    </n-layout>
  </n-layout>
</template>

<style scoped>

</style>