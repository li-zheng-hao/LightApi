<script setup lang="ts">
import TabPage from '@/components/TabPage.vue'
import router from '@/router'
</script>

<template>
  <div class="p-2 w-full h-full">
    <tab-page class="w-full h-full bg-#f5f7f9">
      <h2>这是新的单页</h2>
      <n-button @click="() => router.push('/')">跳转回管理页面</n-button>
    </tab-page>
  </div>
</template>

<style scoped></style>
