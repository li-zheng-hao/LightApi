import { isObject } from "./is";

/**
 * 从src合并到target中并返回合并结果，如果target中已有的对象，则会忽略,不会修改src和target本身
 * @param src 
 * @param target
 */
export function deepMerge<T = any>(src: any = {}, target: any = {}): T {
  const result = { ...src }
  let key: string
  for (key in target) {
    result[key] = isObject(result[key]) ? deepMerge(result[key], target[key]) : (target[key])
  }
  return result
}



/**
 * 深拷贝一个对象
 * @param source
 */
export function deepClone(item:any) {
    if (!item) { return item; } // null, undefined values check

    const types = [ Number, String, Boolean ];
    let result;

    // normalizing primitives if someone did new String('aaa'), or new Number('444');
    types.forEach(function(type) {
        if (item instanceof type) {
            result = type( item );
        }
    });

    if (typeof result == "undefined") {
        if (Object.prototype.toString.call( item ) === "[object Array]") {
            result = [];
            item.forEach(function(child :any, index:number, array:Array<any>) {
                result[index] = deepClone( child );
            });
        } else if (typeof item == "object") {
            // testing that this is DOM
            if (item.nodeType && typeof item.cloneNode == "function") {
                result = item.cloneNode( true );
            } else if (!item.prototype) { // check that this is a literal
                if (item instanceof Date) {
                    result = new Date(item);
                } else {
                    // it is an object literal
                    result = {};
                    for (const i in item) {
                        result[i] = deepClone( item[i] );
                    }
                }
            } else {
                // depending what you would like here,
                // just keep the reference, or create new object
                if (false && item.constructor) {
                    // would not advice to do that, reason? Read below
                    result = new item.constructor();
                } else {
                    result = item;
                }
            }
        } else {
            result = item;
        }
    }

    return result;
}