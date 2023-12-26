<script  lang="ts">

import {defineComponent} from "vue";

export default defineComponent({
  name:'ReloadPage',
 beforeRouteEnter(to, from, next) {
    next(vm => {
      // 通过 `vm` 访问组件实例
      vm.$router.replace(from.fullPath)
    })
  }
})
</script>

<template>
<div>new</div>
</template>

<style scoped>

</style>
