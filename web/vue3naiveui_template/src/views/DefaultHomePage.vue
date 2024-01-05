<script setup lang="ts">
import TabPage from '@/components/TabPage.vue'
import { onMounted, ref } from 'vue'

const onClick = (row: any) => {
  window.$message.success(row.name)
}

onMounted(() => {
  console.log('onMounted')
})
</script>

<template>
  <tab-page>
    <n-date-picker type="date" />
  </tab-page>
</template>

<style scoped></style>
