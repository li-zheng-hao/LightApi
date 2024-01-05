<template>
  <tab-page>
    <div>表格展示页面</div>

    <n-data-table :columns="columns" :data="data" :single-line="false" />
  </tab-page>
</template>
<script setup lang="tsx">
import TabPage from '@/components/TabPage.vue'
type UserInfo = {
  id: number
  name: string
  age: number
  address: string
}
const data: UserInfo[] = [
  { id: 3, name: 'Wonderwall', age: 11, address: '北京' },
  { id: 4, name: "Don't Look Back in Anger", age: 12, address: '上海' },
  { id: 12, name: 'Champagne Supernova', age: 13, address: '海南' }
]

const columns = [
  {
    title: '编号',
    key: 'id'
  },
  {
    title: '姓名',
    key: 'name'
  },
  {
    title: '年龄',
    key: 'age'
  },
  {
    title: '地址',
    key: 'address'
  },
  {
    title: '操作',
    key: 'action',
    render: (row: UserInfo) => {
      return (
        <div class="flex gap-2">
          <n-button type="primary" onClick={() => onClick(row)}>
            查看
          </n-button>
          <n-button onClick={() => onClick(row)}>编辑</n-button>
          <n-button type="warning" onClick={() => onClick(row)}>
            删除
          </n-button>
        </div>
      )
    },
    width: '200px'
  }
]
const onClick = (row: any) => {
  window.$message.success(row.name)
}
</script>
<style scoped></style>
