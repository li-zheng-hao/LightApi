<script setup lang="ts">
import {RouterView} from 'vue-router'
// import { NThemeEditor } from 'naive-ui'
import {NConfigProvider, type GlobalThemeOverrides} from 'naive-ui'
import {lighten} from '@/utils/light'

const lightenStr = lighten('2d8cf0', 6)
const appTheme = '#2d8cf0'
const themeOverrides: GlobalThemeOverrides = {
  common: {
    primaryColor: appTheme,
    primaryColorHover: lightenStr,
    primaryColorPressed: lightenStr,
    primaryColorSuppl: appTheme,
  },
  LoadingBar: {
    colorLoading: appTheme,
  }
}
</script>

<template>
  <n-config-provider :theme-overrides="themeOverrides" class="w-full h-full">
    <RouterView/>
  </n-config-provider>
  <!--  <n-theme-editor>-->
  <!--    <RouterView />-->
  <!--  </n-theme-editor>-->
</template>

<style scoped></style>
