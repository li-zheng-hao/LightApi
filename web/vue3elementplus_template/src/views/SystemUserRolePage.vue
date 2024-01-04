<script setup lang="ts">

import TabPage from "@/components/TabPage.vue";
import router from "@/router";
import {onActivated, onDeactivated, onUnmounted, ref} from "vue";
const count=ref(1)

onUnmounted(()=>{
  console.log('系统用户角色管理页面页面被销毁了')
})
</script>

<template>
  <TabPage>
    系统用户角色管理页面
    <el-button @click="()=>count++">当前点击次数：{{count}}次</el-button>
    <el-button @click="()=>router.push('/')">跳转到主页</el-button>

  </TabPage>
</template>

<style scoped>

</style>
