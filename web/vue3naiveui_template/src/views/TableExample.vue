<template>
  <tab-page>
    <h1>表格展示页面</h1>
    <n-data-table
      :columns="columns"
      :data="data"
      :pagination="pageData"
      remote
      class="h-70vh"
      flex-height
      :loading="isLoading"
      @update:sorter="handleSorterChange"
    />
  </tab-page>
</template>
<script setup lang="tsx">
import { apiClient } from '@/api/client/apiClient'
import type { User } from '@/api/user/info'
import TabPage from '@/components/TabPage.vue'
import { onMounted } from 'vue'
import { ref } from 'vue'

const data = ref<User[]>([])
const isLoading = ref(false)
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
    key: 'age',
    sorter: true
  },
  {
    title: '地址',
    key: 'address'
  },
  {
    title: '操作',
    key: 'action',
    render: (row: User) => {
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
    width: '250px'
  }
]

const pageData = ref({
  page: 1,
  pageCount: 1,
  pageSize: 20,
  showSizePicker: true,
  pageSizes: [3, 20, 50],
  onChange: (page: number) => {
    fetchUserData()
    pageData.value.page = page
  },
  onUpdatePageSize: (pageSize: number) => {
    pageData.value.pageSize = pageSize
    pageData.value.page = 1
    window.$message.info(`切换每页${pageSize}条`)
    fetchUserData()
  }
})

async function fetchUserData() {
  isLoading.value = true
  try {
    const res = await apiClient.request<User[]>({
      url: '/user/info',
      method: 'get'
    })
    data.value = res.slice(
      (pageData.value.page - 1) * pageData.value.pageSize,
      pageData.value.page * pageData.value.pageSize
    )
    pageData.value.pageCount = Math.ceil(res.length / pageData.value.pageSize)
  } finally {
    isLoading.value = false
  }
}
const onClick = (row: any) => {
  window.$message.success(JSON.stringify(row))
}
function handleSorterChange(sorter: any) {
  console.log(sorter)

  fetchUserData()
}
onMounted(() => {
  fetchUserData()
})
</script>
<style scoped></style>
