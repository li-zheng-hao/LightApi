<script setup lang="ts">

import TabPage from "@/components/TabPage.vue";
import {onActivated, onDeactivated, ref} from "vue";
const count=ref(1)

</script>

<template>
  <TabPage>
    系统用户角色管理页面
    <n-button @click="()=>count++">当前点击次数：{{count}}次</n-button>
  </TabPage>
</template>

<style scoped>

</style>
