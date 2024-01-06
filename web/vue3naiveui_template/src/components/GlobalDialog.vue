<script setup lang="ts">
import {useGlobalDialog} from "@/stores/globalDialog";


const globalDialog=useGlobalDialog();


</script>

<template>
  <n-modal
      preset="dialog"
      :content="globalDialog.content"
      v-model:show="globalDialog.isShow"
      :title="globalDialog.title"
      positive-text="确定"
      negative-text="取消"
      :mask-closable="false"
      :on-positive-click="globalDialog.confirm"
      :on-negative-click="globalDialog.cancel"
  >
  </n-modal>
</template>

<style scoped></style>
