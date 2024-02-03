/**
 * 防抖函数
 * @param fn 
 * @param duration 
 * @returns 
 */
export function debounce(fn:(...arg:any[]) => any, duration:number = 300) {
    let timer = -1;
    return function (this:unknown, ...args:any[]) {
      if(timer > -1){
          clearTimeout(timer);
      }
      timer = window.setTimeout(() => {
          fn.bind(this)(...args);
          timer = -1;
      }, duration);
  }
}