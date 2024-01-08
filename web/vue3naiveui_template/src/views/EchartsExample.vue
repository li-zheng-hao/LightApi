<template>
  <div class="flex flex-wrap">
    <div id="table1" class="w-full h-30vh p-2 min-w-500px"></div>
    <div id="table2" class="w-full h-30vh p-2"></div>
  </div>
</template>

<script setup lang="ts">
import echarts from '@/utils/echarts'
import { onMounted, onUnmounted } from 'vue'

let myChart: any

const option = {
  xAxis: {
    data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
  },
  yAxis: {},
  series: [
    {
      type: 'bar',
      data: [23, 24, 18, 25, 27, 28, 25]
    }
  ],
  grid: {
    top: '10px',
    left: '0px',
    right: '0px',
    bottom: '0px',
    containLabel: true
  }
}

onMounted(() => {
  myChart = echarts.init(document.getElementById('table1'), 'shine')

  option && myChart.setOption(option)

  window.addEventListener('resize', function () {
    myChart.resize()
  })
})

onUnmounted(() => {
  window.removeEventListener('resize', function () {
    myChart.resize()
  })
})
</script>

<style scoped></style>
