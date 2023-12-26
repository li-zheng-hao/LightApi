import router from "@/router";
import { formRef } from './LoginView.vue';

export const handleSubmit = () => {
formRef.value?.validate((errors) => {
if (!errors) {
apiClient;
router.push({ path: '/home' });
} else {
window.$message.error("用户名密码填写格式错误");
console.log(errors);
return false;
}
});
};
