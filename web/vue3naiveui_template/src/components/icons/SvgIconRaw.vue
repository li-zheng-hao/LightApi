<template>
    <!-- svg:图标外层容器节点，内部需要与use标签结合使用 -->
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1024 1024">
      <!-- xlink:herf执行用哪一个图标，属性值务必 #icon-图标名字 -->
      <use :xlink:href="prefix + name" ></use>
    </svg>
</template>

<script setup lang="ts">
// 此组件和SvgIcon.vue的区别是，SvgIconRaw.vue用在n-icon组件包裹,所有属性在n-icon上设置
// https://blog.csdn.net/weixin_45232247/article/details/131211236
// 接受父组件传递过来得参数
defineProps({
  // xlink:href属性值的前缀
  prefix: {
    type: String,
    default: '#icon-'
  },
  // svg矢量图的名字
  name: String || null|| undefined,
});
</script>
<style scoped>

</style>
