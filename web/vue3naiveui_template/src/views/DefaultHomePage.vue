<script setup lang="ts">
import TabPage from '@/components/TabPage.vue'
import { onMounted, ref } from 'vue'
import InternalChild from '@/components/InternalChild.vue'
import router from '@/router'
import {useGlobalDialog} from "@/stores/globalDialog";

const onClick = (row: any) => {
  window.$message.success(row.name)
}
const data = ref({
  num: 0
})
function  openDialog(){
  const globalDialog=useGlobalDialog()
  globalDialog.show(
    '标题',
    '内容',
    ()=>{
      window.$message.success('确定')
    },
    ()=>{
      window.$message.error('取消')
    }
  )
}
</script>

<template>
  <tab-page>
    <div class="flex flex-col flex-items-start gap-4">
      <div>默认打开的展示页面</div>

      <n-button @click="openDialog">打开对话框</n-button>
      <internal-child v-model:count="data.num"></internal-child>
      <n-date-picker type="date" />
      <n-button @click="() => router.push('/single')">跳转到单页</n-button>
    </div>
  </tab-page>
</template>

<style scoped></style>
