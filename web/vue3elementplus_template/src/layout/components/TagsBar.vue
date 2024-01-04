<script setup lang="ts">
import { useRouteMenuStore, type RouteItem } from '@/stores/routeMenuStore'
import { computed, onMounted, ref, watch, h, onUnmounted } from 'vue'
import router from '@/router'
import SvgIcon from '@/components/icons/SvgIcon.vue'
import _ from 'lodash'
import { EventBusEvents, EventBus } from '@/utils/eventbus'

const routeMenuStore = useRouteMenuStore()
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
      key: '刷新当前',
      label: '刷新当前'
    },
    {
      key: '关闭当前',
      label: '关闭当前'
    },
    {
      key: '关闭其他',
      label: '关闭其他'
    },
    {
      key: '关闭所有',
      label: '关闭所有'
    }
  ],
  enableScroll: false,
  rightClickItem: null as RouteItem | null
})


watch(
  () => [...routeMenuStore.openedRouteInfo],
  () => {
    updateScroll()
  }
)
const handleSelect = (value,item) => {
  switch (value) {
    case '刷新当前':
      EventBus.emit(EventBusEvents.RELOAD_PAGE)
      break
    case '关闭标签':
      routeMenuStore.removeOpenRoute(item.routePath)
      break
    case '关闭其他':
      routeMenuStore.removeOtherOpenRoute(item?.routePath)
      if (item?.routePath != router.currentRoute.value.path)
        router.push({ path: data.value.rightClickItem?.routePath ?? '/home' })
      break
    case '关闭所有':
      routeMenuStore.removeAllOpenRoute()
      break
  }
  data.value.showDropdown = false
  updateScroll()
}

const updateScrollThrottle = _.throttle(() => updateScroll(), 500)
onMounted(() => {
  window.addEventListener('resize', updateScrollThrottle)
  updateScroll()
})
onUnmounted(() => {
  window.removeEventListener('resize', updateScrollThrottle)
})

const scrollPrev = () => {
  scrollbar.value.scrollLeft -= scrollbar.value.scrollWidth / 10 // adjust scrolling speed
}
const scrollNext = () => {
  scrollbar.value.scrollLeft += scrollbar.value.scrollWidth / 10 // adjust scrolling speed
}

const updateScroll = async () => {
  // await nextTick();
  if (!scrollbar.value) return
  const containerWidth = scrollbar.value.offsetWidth
  const navWidth = scrollbar.value.scrollWidth
  if (containerWidth < navWidth) {
    data.value.enableScroll = true
  } else {
    data.value.enableScroll = false
  }
}


</script>

<template>
  <div class="tags-bar-root-box">
    <div class="flex flex-items-center gap-2">
      <div class="cursor-pointer" @click="scrollPrev" v-if="data.enableScroll">
        <svg-icon name="ArrowBackIosNewRound"></svg-icon>
      </div>

      <div
        class="scrollbar flex flex-items-center gap-2 overflow-x-hidden overflow-y-hidden"
        ref="scrollbar"
      >
        <div v-for="item in routeMenuStore.openedRouteInfo" :key="item.key">
          <el-dropdown trigger="contextmenu">
            <el-tag
              closable
              class="cursor-pointer"
              size="large"
              @click="()=>router.push(item.routePath)"
              @close="()=>onClose(item)"
              :type="router.currentRoute.value.path == item.routePath ? '' : 'info'"
            >
              {{ item.label }}
            </el-tag>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item
                  :disabled="router.currentRoute.value.path !== item.routePath && dropDownOption.key === '刷新当前'"
                  v-for="dropDownOption in data.menuOptions"
                  :key="dropDownOption.key"
                  @click="handleSelect(dropDownOption.key,item)"
                >
                  {{ dropDownOption.label }}
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </div>
      <div class="cursor-pointer" @click="scrollNext" v-if="data.enableScroll">
        <svg-icon name="ArrowForwardIosRound"></svg-icon>
      </div>
    </div>
    <el-dropdown trigger="hover">
      <div class="cursor-pointer">
        <svg-icon name="ArrowDown12Regular"></svg-icon>
      </div>
      <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item
                  v-for="dropDownOption in data.menuOptions"
                  :key="dropDownOption.key"
                  @click="handleSelect(dropDownOption.key,routeMenuStore.currentRouteInfo)"
                >
                  {{ dropDownOption.label }}
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
    </el-dropdown>
  
  </div>
</template>

<style scoped lang="less">
.tags-bar-root-box {
  display: flex;
  align-items: center;
  gap: 10px;

  > div:nth-child(1) {
    width: 98%;
  }

  > div:nth-child(2) {
    width: 2%;
  }
}
</style>
