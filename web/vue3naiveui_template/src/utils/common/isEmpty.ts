/**
 * 判断是否为空
 * @param value
 * @returns
 */
export function isEmpty(value: any) {
  if (value == null) {
    return true
  }
  if (Array.isArray(value) || typeof value === 'string') {
    return !value.length
  }
  if (typeof value === 'object') {
    return !Object.keys(value).length
  }
  return false
}
