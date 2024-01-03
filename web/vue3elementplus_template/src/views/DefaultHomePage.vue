<script setup lang="ts">

import TabPage from "@/components/TabPage.vue";
import { ref } from "vue";

interface RowVO {
  id: number
  name: string
  role: string
  sex: string
  age: number
  address: string
}



</script>

<template>
<tab-page>
  <div>HomePage</div>
</tab-page>
</template>

<style scoped>

</style>
