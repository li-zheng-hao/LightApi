import { PermissionCode } from '@/enums/permissionCode';
import { useUserStore } from '@/stores/user';
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
function hasPermission(codes: string[]) {
  const userSotre=useUserStore();
  return userSotre.permissions.findIndex(it=>{
    for (let index = 0; index < codes.length; index++) {
      const code = codes[index];
      // 拥有指定权限或者是系统管理员权限
      if(it===code || it===PermissionCode.SUPER_ADMIN)
        return true; 
    }
    return false;
  })!=-1;
}