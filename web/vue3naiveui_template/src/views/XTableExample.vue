<script setup lang="ts">
import {XNDataTable, XNDataTableColumn} from "@skit/x.naive-ui";

const data = [
  {id: 1, name: 'Athos', age: 20},
  {id: 2, name: 'Porthos', age: 19},
  {id: 3, name: 'Aramis', age: 18}
];

const onClick = (row: any) => {
  window.$message.success(JSON.stringify(row))
}
</script>
<template>
  <h2>以下使用x.naive-ui插槽写法：</h2>
  <x-n-data-table :data="data" :row-key="row => row.id" :single-line="false" >
    <x-n-data-table-column title="selection" type="selection"/>
    <x-n-data-table-column key="no" title="编号" width="5vw">
      <template #render-cell="{ column, rowData, rowIndex  }">
        <div >{{ rowIndex+1 }}</div>
      </template>
    </x-n-data-table-column>
    <x-n-data-table-column key="name" title="名字"/>
    <x-n-data-table-column key="age" title="年龄"/>
    <x-n-data-table-column key="actions" title="操作" width="15vw">
      <template #render-cell="{ column, rowData, rowIndex }">
        <div class="flex gap-2">
          <n-button type="primary" size="small" @click="onClick(rowData)">查看</n-button>
          <n-button type="warning" size="small"  @click="onClick(rowData)">编辑</n-button>
          <n-button type="error" size="small"  @click="onClick(rowData)">删除</n-button>
        </div>
      </template>
    </x-n-data-table-column>
  </x-n-data-table>
</template>
