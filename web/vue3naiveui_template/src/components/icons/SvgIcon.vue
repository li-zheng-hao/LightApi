<template>
    <!-- svg:图标外层容器节点，内部需要与use标签结合使用 -->
    <svg class="svg-node flex flex-items-center" :style="{ width: size+'px',height:size+'px',scale:scale,color:color}">
      <!-- xlink:herf执行用哪一个图标，属性值务必 #icon-图标名字 -->
      <use :xlink:href="prefix + name" ></use>
    </svg>
</template>

<script setup>
// https://blog.csdn.net/weixin_45232247/article/details/131211236
// 接受父组件传递过来得参数
defineProps({
  // xlink:href属性值的前缀
  prefix: {
    type: String,
    default: '#icon-'
  },
  // svg矢量图的名字
  name: String,
  // svg图标的颜色
  color: {
    type: String,
    default: null
  },
  // // svg宽度
  // width: {
  //   type: String,
  //   default: '18px'
  // },
  // // svg高度
  // height: {
  //   type: String,
  //   default: '18px'
  // },
  size:{
    type: Number,
    default: 18
  },
  // svg缩放比例
  scale: {
    type: Number,
    default: 1
  }
});
</script>
<style scoped>

</style>
