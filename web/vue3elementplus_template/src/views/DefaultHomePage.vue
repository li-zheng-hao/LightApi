<script setup lang="ts" name="DefaultHomePage">

import TabPage from "@/components/TabPage.vue";
import router from "@/router";
import { onMounted, onUnmounted, ref } from "vue";

interface RowVO {
  id: number
  name: string
  role: string
  sex: string
  age: number
  address: string
}
onUnmounted(()=>{
  console.log('主页被销毁了')
})

</script>

<template>
<tab-page>
  <div>HomePage</div>
  <ElButton>Click123</ElButton>
  <el-tag>Tag 1</el-tag>
  <el-tag class="ml-2" type="success">Tag 2</el-tag>
  <el-tag class="ml-2" type="info">Tag 3</el-tag>
  <el-tag class="ml-2" type="warning">Tag 4</el-tag>
  <el-tag class="ml-2" type="danger">Tag 5</el-tag>
</tab-page>
</template>

<style scoped>

</style>
