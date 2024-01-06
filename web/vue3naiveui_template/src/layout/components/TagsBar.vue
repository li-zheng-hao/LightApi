<script setup lang="ts">
import {useRouteMenuStore, type MenuItem} from '@/stores/routeMenuStore'
import {onMounted, ref, watch, h, onUnmounted, nextTick} from 'vue'
import router from '@/router'
import SvgIcon from '@/components/icons/SvgIcon.vue'
import {NIcon} from 'naive-ui'
import {_} from '@/utils/common'
import {EventBus, EventBusEvents} from '@/utils/eventbus'
import SvgIconRaw from "@/components/icons/SvgIconRaw.vue";

const routeMenuStore = useRouteMenuStore()
const onClose = (item) => {
  routeMenuStore.removeOpenTab(item.routePath)
}

const onChangeRoute = (item) => {
  router.push({path: item.routePath})
}
const scrollbar = ref(null)

const data = ref({
  xPos: 0,
  yPos: 0,
  showDropdown: false,
  menuOptions: [
    {
      key: '刷新当前',
      label: '刷新当前',
      icon() {
        return h(NIcon, null, {
          default: () => h(SvgIcon, {name: 'ReloadOutlined'})
        })
      },
      disabled: false
    },
    {
      key: '关闭选择',
      label: '关闭选择',
      icon() {
        return h(NIcon, null, {
          default: () => h(SvgIcon, {name: 'IosClose', scale: 1.4})
        })
      },
      disabled: false
    },
    {
      key: '关闭其他',
      label: '关闭其他',
      icon() {
        return h(NIcon, null, {
          default: () => h(SvgIcon, {name: 'CloseCircleOutlined'})
        })
      }
    },
    {
      key: '关闭所有',
      label: '关闭所有',
      icon() {
        return h(NIcon, null, {
          default: () => h(SvgIcon, {name: 'ExclamationOutlined'})
        })
      }
    }
  ],
  enableScroll: false,
  rightClickItem: null as MenuItem | null,
  enableReloadCurrentPage: true
})

const onClickOutside = () => {
  data.value.showDropdown = false
  data.value.menuOptions[0].disabled = false
}
const handleContextMenu = (e: MouseEvent, item: any) => {
  e.preventDefault()
  data.value.rightClickItem = item
  data.value.showDropdown = false
  if (item && item.routePath != router.currentRoute.value.fullPath) {
    data.value.menuOptions[0].disabled = true
  }
  nextTick().then(() => {
    data.value.showDropdown = true
    data.value.xPos = e.clientX
    data.value.yPos = e.clientY
  })
}
watch(
    () => [...routeMenuStore.openedTabs],
    () => {
      updateScroll()
    }
)
const handleSelect = (value: any) => {
  switch (value) {
    case '刷新当前':
      EventBus.emit(EventBusEvents.RELOAD_PAGE)
      break
    case '关闭选择':
      routeMenuStore.removeOpenTab(data.value.rightClickItem?.routePath)
      break
    case '关闭其他':
      const deleteCurRoutePath =
          data.value.rightClickItem?.routePath ?? router.currentRoute.value.fullPath
      routeMenuStore.removeOtherOpenTabs(deleteCurRoutePath)

      if (deleteCurRoutePath != router.currentRoute.value.fullPath)
        router.push({path: data.value.rightClickItem?.routePath ?? '/'})
      break
    case '关闭所有':
      routeMenuStore.removeAllOpenTabs()
      router.push({path: '/home'})
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
  await nextTick()
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
        <n-tag
            closable
            class="cursor-pointer bg-white h-8 p-2 flex-shrink-0"
            v-for="item in routeMenuStore.openedTabs"
            :type="router.currentRoute.value.path == item.routePath ? 'info' : ''"
            @click="onChangeRoute(item)"
            @close="onClose(item)"
            @contextmenu="handleContextMenu($event, item)"
            :key="item.key"
        >
          {{ item?.label }}
        </n-tag>
      </div>
      <div class="cursor-pointer" @click="scrollNext" v-if="data.enableScroll">
        <svg-icon name="ArrowForwardIosRound"></svg-icon>
      </div>
    </div>
    <div class="cursor-pointer" @click="handleContextMenu($event,null)">
      <svg-icon name="ArrowDown12Regular" :scale="1.2"></svg-icon>
    </div>
    <n-dropdown
        placement="bottom-start"
        trigger="manual"
        :x="data.xPos"
        :y="data.yPos"
        :options="data.menuOptions"
        :show="data.showDropdown"
        :on-clickoutside="onClickOutside"
        @select="handleSelect"
    ></n-dropdown>
  </div>
</template>

<style scoped lang="less">
.tags-bar-root-box {
  display: flex;
  align-items: center;
  gap: 10px;
  justify-content: space-between;
  padding-right: 1rem;

  > div:nth-child(1) {
    overflow-x: hidden;
  }

}
</style>
