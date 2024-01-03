<script setup lang="ts">
import { useRouteMenuStore, type RouteItem } from "@/stores/routeMenuStore";
import { computed, onMounted, ref, watch, h, onUnmounted } from "vue";
import router from "@/router";
import SvgIcon from "@/components/icons/SvgIcon.vue";
import _ from "lodash";

const routeMenuStore = useRouteMenuStore();
const onClose = (item) => {
  routeMenuStore.removeOpenRoute(item.routePath)
}
const onChangeRoute = (item) => {
  router.push({ path: item.routePath })
}

const scrollbar = ref(null)

const data = ref({
  xPos: 0,
  yPos: 0,
  showDropdown: false,
  menuOptions: [
    {
      key: '刷新当前', label: '刷新当前',
    },
    {
      key: '关闭当前', label: '关闭当前'
    },
    {
      key: '关闭其他', label: '关闭其他'
    },
    {
      key: '关闭所有', label: '关闭所有',
    },
  ],
  enableScroll: false,
  rightClickItem: null as RouteItem | null

})

const onClickoutside = () => {
  data.value.showDropdown = false
}
const handleContextMenu = (e: MouseEvent,item:any) => {
  e.preventDefault()
  data.value.rightClickItem=item
  data.value.showDropdown = false
  // nextTick().then(() => {
  //   data.value.showDropdown = true
  //   data.value.xPos = e.clientX
  //   data.value.yPos = e.clientY
  // })
}
watch(() => [...routeMenuStore.openedRouteInfo], () => {
  updateScroll()
})
const handleSelect = (value) => {
  switch (value) {
    case '刷新当前':
      router.push({ path: '/reload' })
      break
    case '关闭当前':
      routeMenuStore.removeOpenRoute(data.value.rightClickItem?.routePath)
      break
    case '关闭其他':
      routeMenuStore.removeOtherOpenRoute(data.value.rightClickItem?.routePath)
      if(data.value.rightClickItem?.routePath!=router.currentRoute.value.path) router.push({path:data.value.rightClickItem?.routePath??"/" })
      break
    case '关闭所有':
      routeMenuStore.removeAllOpenRoute()
      router.push({ path: '/' })
      break
  }
  data.value.showDropdown = false
  updateScroll()

}

const updateScrollThrottle = _.throttle(() => updateScroll(), 500)
onMounted(() => {
  window.addEventListener('resize', (updateScrollThrottle))
  updateScroll()
})
onUnmounted(() => {
  window.removeEventListener('resize', updateScrollThrottle)
})

const scrollPrev = () => {
  scrollbar.value.scrollLeft -= (scrollbar.value.scrollWidth / 10); // adjust scrolling speed
}
const scrollNext = () => {
  scrollbar.value.scrollLeft += (scrollbar.value.scrollWidth / 10); // adjust scrolling speed
}

const updateScroll = async () => {
  // await nextTick();
  if (!scrollbar.value) return;
  const containerWidth = scrollbar.value.offsetWidth;
  const navWidth = scrollbar.value.scrollWidth;
  if (containerWidth < navWidth) {
    data.value.enableScroll = true;
  } else {
    data.value.enableScroll = false;
  }
}


</script>

<template>
  <div class="tags-bar-root-box">
    <div class="flex flex-items-center gap-2">
      <div class="cursor-pointer" @click="scrollPrev" v-if="data.enableScroll">
        <svg-icon name="ArrowBackIosNewRound"></svg-icon>
      </div>
      <div class="scrollbar flex flex-items-center gap-2 overflow-x-hidden overflow-y-hidden" ref="scrollbar">
<!--        <n-tag closable class="cursor-pointer bg-white h-8 p-2 flex-shrink-0"-->
<!--          v-for="item in  routeMenuStore.openedRouteInfo" :type="router.currentRoute.value.path == item.routePath ? 'info' : ''"-->
<!--          @click="onChangeRoute(item)" @close="onClose(item)" @contextmenu="handleContextMenu($event,item)">-->
<!--          {{ item?.label }}-->
<!--        </n-tag>-->
      </div>
      <div class="cursor-pointer" @click="scrollNext" v-if="data.enableScroll">
        <svg-icon name="ArrowForwardIosRound"></svg-icon>
      </div>

    </div>
    <div class="cursor-pointer" @mouseenter="handleContextMenu">
      <svg-icon name="ArrowDown12Regular"></svg-icon>
    </div>
<!--    <n-dropdown placement="bottom-start" trigger="manual" :x="data.xPos" :y="data.yPos" :options="data.menuOptions"-->
<!--      :show="data.showDropdown" :on-clickoutside="onClickoutside" @select="handleSelect"></n-dropdown>-->

  </div>
</template>

<style scoped lang="less">
.tags-bar-root-box {
  display: flex;
  align-items: center;
  gap: 10px;

  >div:nth-child(1) {
    width: 98%;
  }

  >div:nth-child(2) {
    width: 2%;
  }
}
</style>
