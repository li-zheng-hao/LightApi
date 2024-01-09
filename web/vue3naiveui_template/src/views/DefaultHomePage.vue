<script setup lang="ts">
import TabPage from '@/components/TabPage.vue'
import {onMounted, ref} from 'vue'
import InternalChild from '@/components/InternalChild.vue'
import router from '@/router'
import {apiClient} from "@/api/client/apiClient";
import type {User} from "@/api/user/info";

const data = ref({
  num: 0
})

onMounted(async () => {
  const res = await apiClient.request<any>({
        url: '/user',
        method: 'get'
      })
  window.$notification.info({
    title: '请求成功',
    description: JSON.stringify(res),
    duration: 3000
  })

})


async function openDialog() {
  for (let index = 0; index < 10; index++) {
      await apiClient.request<any>({
        url: '/user/info',
        method: 'get'
      })    
  }
 
}
</script>

<template>
  <tab-page>
    <div class="flex flex-col flex-items-start gap-4">
      <div>默认打开的展示页面</div>
      <n-button @click="openDialog">防重复调用测试10次，控制台可以查看只有1次</n-button>
      <internal-child v-model:count="data.num"></internal-child>
      <n-date-picker type="date"/>
      <n-button @click="() => router.push('/single')">跳转到单页</n-button>
    </div>
  </tab-page>
</template>

<style scoped></style>
