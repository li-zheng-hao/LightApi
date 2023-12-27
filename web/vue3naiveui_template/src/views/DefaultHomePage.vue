<script setup lang="ts">

import TabPage from "@/components/TabPage.vue";
import { ref } from "vue";
import { VxeTable,VxeColumn,type VxeTablePropTypes } from 'vxe-table'

interface RowVO {
  id: number
  name: string
  role: string
  sex: string
  age: number
  address: string
}

const allAlign = ref<VxeTablePropTypes.Align>(null)

const tableData = ref<RowVO[]>([
  { id: 10001, name: 'Test1', role: 'Develop', sex: 'Man', age: 28, address: 'test abc' },
  { id: 10002, name: 'Test2', role: 'Test', sex: 'Women', age: 22, address: 'Guangzhou' },
  { id: 10003, name: 'Test3', role: 'PM', sex: 'Man', age: 32, address: 'Shanghai' },
  { id: 10004, name: 'Test4', role: 'Designer', sex: 'Women', age: 24, address: 'Shanghai' }
])
</script>

<template>
<tab-page>
  <vxe-table
      :align="allAlign"
      :data="tableData">
      <vxe-column type="seq" width="60" title="序号"> </vxe-column>
      <vxe-column field="name" title="名称"></vxe-column>
      <vxe-column field="sex" title="性别"></vxe-column>
      <vxe-column field="age" title="年龄"></vxe-column>
    </vxe-table>
</tab-page>
</template>

<style scoped>

</style>
