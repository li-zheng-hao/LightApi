/**
 * 全屏服务
 * @returns
 */
function useScreen() {
  /**
   * 改变浏览器全屏模式
   */
  function changeFullScreenState() {
    if (isFullScreen()) {
      exitFullScreen()
    } else {
      enterFullScreen()
    }
  }
  /**
   * 进入全屏模式
   */
  function enterFullScreen() {
    const docElm = document.documentElement
    if (docElm.requestFullscreen) {
      docElm.requestFullscreen()
    } else if (docElm.msRequestFullscreen) {
      docElm.msRequestFullscreen()
    } else if (docElm.mozRequestFullScreen) {
      docElm.mozRequestFullScreen()
    } else if (docElm.webkitRequestFullScreen) {
      docElm.webkitRequestFullScreen()
    }
  }
  /**
   * 推出浏览器全屏模式
   */
  function exitFullScreen() {
    if (document.exitFullscreen) {
      document.exitFullscreen()
    } else if (document.msExitFullscreen) {
      document.msExitFullscreen()
    } else if (document.mozCancelFullScreen) {
      document.mozCancelFullScreen()
    } else if (document.webkitCancelFullScreen) {
      document.webkitCancelFullScreen()
    }
  }
  /**
   * 判断浏览器是否处于全屏状态 （需要考虑兼容问题）
   * @returns
   */
  function isFullScreen() {
    //火狐浏览器
    let isFull =
      document.mozFullScreen ||
      document.fullScreen ||
      document.webkitIsFullScreen ||
      document.webkitRequestFullScreen ||
      document.mozRequestFullScreen ||
      document.msFullscreenEnabled
    if (isFull === undefined) isFull = false
    return isFull
  }

  return { isFullScreen, changeFullScreenState, setFullScreen: enterFullScreen, exitFullScreen }
}
