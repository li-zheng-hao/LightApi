import { type ObjectDirective } from 'vue';

export const permission: ObjectDirective = {
  mounted(el: HTMLButtonElement, binding) {
    if (binding.value == undefined) return;
    const { code, effect } = binding.value;
    if (!hasPermission(code)) {
      if (effect == 'disable') {
        el.disabled = true;
        el.style['disabled' as any] = 'disabled';
        // el.classList.add('n-button--disabled');
        el.classList.add(el.classList[0] + '--disabled');
      } else {
        el.remove();
      }
    }
  }
};
// 这里要根据具体的用户权限修改
function hasPermission(code: string[]) {
  return code.includes('admin');
}