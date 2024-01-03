<script setup lang="ts">
import {RouterView} from 'vue-router'
import { ElConfigProvider } from 'element-plus'
import zhCn from 'element-plus/dist/locale/zh-cn.mjs'
</script>

<template>
  <el-config-provider :locale="zhCn">
        <RouterView/>
  </el-config-provider>
</template>

<style lang="less">

</style>
