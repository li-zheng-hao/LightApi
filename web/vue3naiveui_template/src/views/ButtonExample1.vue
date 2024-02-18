<script setup lang="ts">
import TabPage from '@/components/TabPage.vue'
import { useUserStore } from '@/stores/user';
import { NButton, NDatePicker } from 'naive-ui';

const userStore=useUserStore()

const click = () => {
  window.$message.success('Hello')
}
function onClick2() {
  window.$message.success('点击')
  
}

</script>

<template>
  <tab-page>
    <div><n-button @click="click">Example1</n-button></div>
    <n-button v-permission="{ code:['admin'],effect:'disable'}" @click="onClick2">权限管理 只有有管理员权限的能点击</n-button>
    <NDatePicker :disabled="!userStore.hasPermission(['admin'])" ></NDatePicker>
  </tab-page>
</template>

<style scoped></style>
