<script setup lang="ts">
import TabPage from '@/components/TabPage.vue'
import { NButton, NDatePicker } from 'naive-ui';
const click = () => {
  window.$message.success('Hello')
}
function onClick2() {
}
</script>

<template>
  <tab-page>
    <div><n-button @click="click">Example1</n-button></div>
    <n-button v-permission="{ code:['admin'],effect:'disable'}" @click="onClick2">权限管理 只有有管理员权限的能点击</n-button>
    <NDatePicker v-permission="{ code:['admin123']}" :disabled="true"></NDatePicker>
  </tab-page>
</template>

<style scoped></style>
