<script setup lang="ts">
import TabPage from '@/components/TabPage.vue'
import { useRouteMenuStore } from '@/stores/menuStore';
import { onMounted } from 'vue';
const routeMenuStore=useRouteMenuStore();
onMounted(() => {
    routeMenuStore.setCurrentBreadcrumbInfo(['个人设置'])
  
});
</script>
<template>
  <TabPage class="user-setting-box">
    <h1>个人设置</h1>
  </TabPage>
</template>

<style scoped lang="less">

.user-setting-box{
    min-height: 800px;
}
</style>
