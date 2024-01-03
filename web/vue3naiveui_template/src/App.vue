<script setup lang="ts">
import {RouterView} from 'vue-router'
// import { NThemeEditor } from 'naive-ui'
import {NConfigProvider, type GlobalThemeOverrides,zhCN, dateZhCN } from 'naive-ui'
import {lighten} from '@/utils/light'
import {onMounted} from "vue";
import MessageApi from "@/components/MessageApi.vue";
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
  <n-config-provider :theme-overrides="themeOverrides" :locale="zhCN" :date-locale="dateZhCN" class="w-full h-full">
    <n-notification-provider>
      <n-message-provider>
        <message-api></message-api>
        <RouterView/>
      </n-message-provider>
    </n-notification-provider>
  </n-config-provider>
  <!--  <n-theme-editor>-->
  <!--    <RouterView />-->
  <!--  </n-theme-editor>-->
</template>

<style scoped>


</style>
