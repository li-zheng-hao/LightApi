<script setup lang="ts">
import BasicLayout from "@/layout/BasicLayout.vue";
import { onMounted,ref } from "vue";
import axios from "axios";
import SvgIcon from "@/components/icons/SvgIcon.vue";
const users=ref({})
onMounted(async () => {
  users.value=await axios.get('/api/user')
});
</script>

<template>
  <basic-layout>
  </basic-layout>
  <svg-icon name="order"></svg-icon>
</template>
