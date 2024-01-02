<script setup lang="ts">

import TabPage from "@/components/TabPage.vue";
const click=()=>{
  window.$message.success('Hello')
}
</script>

<template>
  <tab-page>
    <div><n-button @click="click">Example1</n-button></div>

  </tab-page>
</template>

<style scoped>

</style>
