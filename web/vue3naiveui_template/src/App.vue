<script setup lang="ts">
import { RouterView } from 'vue-router'
import { NConfigProvider, type GlobalThemeOverrides, zhCN, dateZhCN } from 'naive-ui'
import { lighten } from '@/utils/light'
import {useDesignSettingStore} from "@/stores/designSettingStore";
const designSettingStore=useDesignSettingStore();
const themeOverrides: GlobalThemeOverrides = {
  common: {
    primaryColor: designSettingStore.appTheme,
    primaryColorHover: lighten(designSettingStore.appTheme, 6),
    primaryColorPressed: lighten(designSettingStore.appTheme, 6),
    primaryColorSuppl: designSettingStore.appTheme
  },
  LoadingBar: {
    colorLoading: designSettingStore.appTheme
  }
}
</script>

<template>
  <n-config-provider
    :theme-overrides="themeOverrides"
    :locale="zhCN"
    :date-locale="dateZhCN"
    class="w-full h-full"
  >
    <n-notification-provider>
      <n-message-provider>
        <RouterView />
      </n-message-provider>
    </n-notification-provider>
  </n-config-provider>
</template>

<style scoped></style>
