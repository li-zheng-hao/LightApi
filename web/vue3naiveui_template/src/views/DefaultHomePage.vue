<script setup lang="ts">

import TabPage from "@/components/TabPage.vue";
import { NButton, NTag } from "naive-ui";
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

const allAlign = ref<VxeTablePropTypes.Align>('left')

const tableData = ref<RowVO[]>([
  { id: 10001, name: 'Test1', role: 'Develop', sex: 'Man', age: 28, address: 'test abc' },
  { id: 10002, name: 'Test2', role: 'Test', sex: 'Women', age: 22, address: 'Guangzhou' },
  { id: 10003, name: 'Test3', role: 'PM', sex: 'Man', age: 32, address: 'Shanghai' },
  { id: 10004, name: 'Test4', role: 'Designer', sex: 'Women', age: 24, address: 'Shanghai' }
])
const onClick=(row:any)=>{
  window.$message.success(row.name)
}
</script>

<template>
<tab-page>
  <vxe-table
      :align="allAlign"
      :border="true"
      :data="tableData">
      <vxe-column type="seq" width="60" title="序号"> </vxe-column>
      <vxe-column field="name" title="名称">
        <template #default="{row}">
           <NTag type="info"> {{ row.name }}</NTag>
        </template>
      </vxe-column>
      <vxe-column field="sex" title="性别"></vxe-column>
      <vxe-column field="age" title="年龄"></vxe-column>
      <vxe-column>
        <template #default="{row}">
          <NButton @click="onClick(row)">选择</NButton>
        </template>
      </vxe-column>
    </vxe-table>
    <n-date-picker type="date" />

</tab-page>
</template>

<style scoped>

</style>
