import { DialogApi, MessageApi, NotificationApi } from 'naive-ui'
import { LoadingBarInst } from 'naive-ui/es/loading-bar/src/LoadingBarProvider'

declare global {
  interface Window {
    $message: MessageApi
    $dialog: DialogApi
    $notification: NotificationApi
    $loading: LoadingBarInst
  }
}
