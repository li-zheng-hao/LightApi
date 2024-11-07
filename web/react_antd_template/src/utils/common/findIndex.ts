/**
 * 从数组中找到第一个满足条件的元素 没有返回-1
 * @param array 数组
 * @param predicate 过滤条件
 * @param fromIndex 从什么元素开始 不传为-1
 * @returns
 */
export function findIndex<T>(
  array: T[],
  predicate: (value: T, index: number, obj: T[]) => unknown
): number {
  const length = array == null ? 0 : array.length;
  if (!length) {
    return -1;
  }
  return array.findIndex((item, i) => predicate(item, i, array));
}
