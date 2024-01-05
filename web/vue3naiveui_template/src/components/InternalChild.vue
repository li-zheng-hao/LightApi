<template>
  <h1>{{ props.count }}</h1>
  <n-button @click="addCount">点击改变父组件的值</n-button>
</template>
<script setup lang="ts">
const props = defineProps({
  count: { type: Number, required: true }
})
const emit = defineEmits(['update:count'])
const addCount: () => void = () => {
  emit('update:count', props.count + 1)
}
</script>
