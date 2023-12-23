<script lang="ts" setup>
import { defineComponent, h, ref, Component } from 'vue'
import { NIcon } from 'naive-ui'
import type { MenuOption } from 'naive-ui'
import IconCommunity from "@/components/icons/IconCommunity.vue";
function renderIcon (icon: Component) {
  return () => h(NIcon, null, { default: () => h(icon) })
}
const menuOptions: MenuOption[] = [
  {
    label: '且听风吟',
    key: 'hear-the-wind-sing',
    icon: renderIcon(IconCommunity)
  },
  {
    label: '1973年的弹珠玩具',
    key: 'pinball-1973',
    icon: renderIcon(IconCommunity),
    disabled: true,
    children: [
      {
        label: '鼠',
        key: 'rat'
      }
    ]
  },
  {
    label: '寻羊冒险记',
    key: 'a-wild-sheep-chase',
    disabled: true,
    icon: renderIcon(IconCommunity)
  },
  {
    label: '舞，舞，舞',
    key: 'dance-dance-dance',
    icon: renderIcon(IconCommunity),
    children: [
      {
        type: 'group',
        label: '人物',
        key: 'people',
        children: [
          {
            label: '叙事者',
            key: 'narrator',
            icon: renderIcon(IconCommunity)
          },
          {
            label: '羊男',
            key: 'sheep-man',
            icon: renderIcon(IconCommunity)
          }
        ]
      },
      {
        label: '饮品',
        key: 'beverage',
        icon: renderIcon(IconCommunity),
        children: [
          {
            label: '威士忌',
            key: 'whisky'
          }
        ]
      },
      {
        label: '食物',
        key: 'food',
        children: [
          {
            label: '三明治',
            key: 'sandwich'
          }
        ]
      },
      {
        label: '过去增多，未来减少',
        key: 'the-past-increases-the-future-recedes'
      }
    ]
  }
]

const data=ref<any>({
  activeKey: null,
  collapsed: false,
  menuOptions
})
const props=defineProps(['collapsed'])
</script>

<template>
      <n-menu
          v-model:value="data.activeKey"
          :collapsed="props.collapsed"
          :collapsed-width="64"
          :collapsed-icon-size="22"
          :options="data.menuOptions"
      />
</template>

<style scoped>

</style>