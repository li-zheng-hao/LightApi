/// <reference types="vite/client" />

import { DialogApi, MessageApi, NotificationApi } from 'naive-ui'
import { LoadingBarInst } from 'naive-ui/es/loading-bar/src/LoadingBarProvider'

declare module '*.vue' {
  import { DefineComponent } from 'vue'
  const component: DefineComponent<{}, {}, any>
  export default component
}

