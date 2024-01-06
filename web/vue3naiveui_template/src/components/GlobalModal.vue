<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { EventBus, EventBusEvents } from '@/utils/eventbus'
interface ModalConfig {
  title: string
  content: string
  show: boolean
  positiveText: string
  negativeText: string
}

const config = ref<ModalConfig>({
  title: '标题',
  content: '内容',
  show: false,
  positiveText: '确定',
  negativeText: '取消'
} as ModalConfig)

onMounted(() => {
  EventBus.on(EventBusEvents.SHOW_MODAL, (config: ModalConfig) => {
    config.show = true
  })
})
</script>

<template>
  <n-modal
    preset="dialog"
    :content="config.content"
    v-model:show="config.show"
    :title="config.title"
    :positive-text="config.positiveText"
    :negative-text="config.negativeText"
    :mask-closable="false"
  >
  </n-modal>
</template>

<style scoped></style>
