import mitt from 'mitt'

export enum EventBusEvents {
  // 刷新当前页面
  RELOAD_PAGE = 'reload-page'
}

export const EventBus = mitt()
