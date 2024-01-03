<script setup lang="ts">
import { ref } from 'vue'
import SvgIcon from "@/components/icons/SvgIcon.vue";
import router from "@/router";
import { apiClient } from '@/api/client/request';
const formInline = ref({
  username: '',
  password: '',
})
const rules = ref({
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
  ],
})
const formRef = ref(null)
const handleSubmit = () => {
  formRef.value?.validate(async (errors) => {
    if (!errors) {
      let apiResult = await apiClient.post('/user/login', formInline.value)
      router.push({ path: '/home' })
    } else {
      window.$message.error("用户名密码填写格式错误")
      return false
    }
  })
}
const loading = ref(false)
</script>

<template>
  <div class="login-box w-100vw h-100vh flex flex-col">
    <div class="login-title flex flex-items-center mb-4 gap-2">
      <n-avatar color="#2d8cf0">
        LZH
      </n-avatar>
      <span class="text-4xl">Vue3模板</span>
    </div>
    <div class="mb-4 color-gray">小小的项目描述</div>
    <n-form ref="formRef" label-placement="left" class="login-form" size="large" :model="formInline" :rules="rules">
      <n-form-item path="username">
        <n-input v-model:value="formInline.username" placeholder="请输入用户名">
          <template #prefix>
            <n-icon size="18" color="#808695">
              <svg-icon name="UserOutlined"></svg-icon>
            </n-icon>
          </template>
        </n-input>
      </n-form-item>
      <n-form-item path="password">
        <n-input v-model:value="formInline.password" type="password" showPasswordOn="click" placeholder="请输入密码">
          <template #prefix>
            <n-icon size="18" color="#808695">
              <SvgIcon name="LockTwotone" />
            </n-icon>
          </template>
        </n-input>
      </n-form-item>

      <n-form-item>
        <el-button type="primary" @click="handleSubmit" size="large" :loading="loading" block>
          登录
        </el-button>
      </n-form-item>
      <n-form-item class="default-color">
        <div class="flex view-account-other">
          <div class="flex-initial">
            <span>其它登录方式</span>
          </div>
          <div class="flex-initial mx-4">
            <a href="javascript:">
              <svg-icon name="QqSquareFilled" :size="24" color="#2d8cf0"></svg-icon>
            </a>
          </div>
          <div class="flex-initial" style="margin-left: auto">
            <a href="javascript:">注册账号</a>
          </div>
          <div class="flex justify-between">
            <div class="flex-initial order-last">
              <a href="javascript:">忘记密码</a>
            </div>
          </div>
        </div>
      </n-form-item>
    </n-form>
  </div>
</template>

<style scoped lang="less">
.login-box {
  background-image: url('@/assets/images/login.svg');
  background-repeat: no-repeat;
  background-position: 50%;
  background-size: 100%;
  display: flex;
  align-items: center;
  margin-top: 15vh;
}

.login-form {
  width: 400px;
}
</style>
