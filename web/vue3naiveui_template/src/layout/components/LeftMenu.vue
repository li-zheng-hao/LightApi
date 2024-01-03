<script lang="tsx" setup>
import {defineComponent, h, ref, type Component, onMounted} from 'vue'
import { NIcon } from 'naive-ui'
import type { MenuOption } from 'naive-ui'
import { useRouteMenuStore} from "@/stores/routeMenuStore";
import SvgIcon from "@/components/icons/SvgIcon.vue";
import {RouterLink} from "vue-router";
import _, { divide } from 'lodash'
import router from "@/router";
function renderIcon (icon: string|null|undefined) {
  if (!icon) return null
  return () => h(SvgIcon,{ name:icon})
}
const routeMenuStore=useRouteMenuStore()

const data=ref<any>({
  collapsed: false,
  menuOptions: [] as MenuOption[],
  expandKeys: ['/sys/menu'],
})

const handleUpdateRoute=(key:string)=>{
  routeMenuStore.addOpenRoute(key)
}

onMounted(()=>{
  let routeInfo=routeMenuStore.getAllRouteInfo()
  // 注册最多三级菜单
  routeInfo.forEach((value,key)=>{
    data.value.menuOptions.push({
      label: value.children?.length??0>0? ()=><div>{value.label}</div>: ()=> <RouterLink to={value.routePath??""}>{value.label}</RouterLink>,
      key: value.key,
      icon: ()=><SvgIcon name={value.icon}></SvgIcon>
    })
    if(_.isEmpty(value.children)) return;
    data.value.menuOptions[key].children=[]

    value.children?.forEach((child,childKey)=>{
      data.value.menuOptions[key].children?.push({
        label: ()=> <RouterLink to={child.routePath??""}>{child.label}</RouterLink>,
        key: child.key,
        icon: renderIcon(child.icon),
      })

      if(_.isEmpty(child.children)) return;
      data.value.menuOptions[key].children[childKey].children=[]
      child.children?.forEach((grandson,grandsonKey)=>{
        data.value.menuOptions[key].children[childKey].children.push({
          label: ()=>
              h(
                  RouterLink,
                  {
                    to: {
                      path: grandson.routePath??"",
                    }
                  },
                  { default: () => grandson.label }
              ),
          key: grandson.key,
          icon: renderIcon(grandson.icon),
        })
      })
    })


  })

  routeMenuStore.addOpenRoute(router.currentRoute.value.path)
})



const props=defineProps(['collapsed'])
</script>

<template>
      <n-menu
          :inverted="true"
          v-model:value="routeMenuStore.currentRouteInfo!.routePath"
          :collapsed="routeMenuStore.menuCollapsed"
          :collapsed-width="64"
          :options="data.menuOptions"
          :accordion="true"
          :indent="20"
          :watch-props="['defaultExpandedKeys']"
          @update:value="handleUpdateRoute"
      />
</template>

<style scoped>

</style>
