/**
 * 节流函数
 * @param fn 
 * @param interval 
 * @returns 
 */
export function throttle(fn:(...arg:any[]) => any, interval:number = 300) {
    let lock = false;
    return function (this:unknown, ...args:any[]){
      if(lock) return;
      lock = true;
      setTimeout(() => lock = false, interval);
      fn.bind(this)(...args);
    }
  }