<script setup lang="ts">
import {RouterView} from 'vue-router'
import { ElConfigProvider } from 'element-plus'
import zhCn from 'element-plus/dist/locale/zh-cn.mjs'
import { onMounted }from 'vue'
import { changeElementPlusTheme,defaultElementPlusCustomThemeConfig } from './config/elementplusTheme';


  
onMounted(()=>{
  changeElementPlusTheme(defaultElementPlusCustomThemeConfig)
})


</script>

<template>
  <el-config-provider :locale="zhCn">
        <RouterView/>
  </el-config-provider>
</template>

<style lang="less" scoped>
:deep(.el-tag .el-icon){
  border-radius: 0%;
}
:deep(.el-tag){
  --el-tag-border-radius:2px;
}
</style>
