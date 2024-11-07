import { App, Button, Form, Input } from 'antd';
import { useNavigate } from 'react-router-dom';
import styles from './Login.module.scss';
import { LockOutlined, UserOutlined } from '@ant-design/icons';
import { apiClient } from '../api/client/apiClient.ts';
import userStore from '../stores/UserStore.ts';
type LoginType = {
  username?: string;
  password?: string;
};

const LoginPage = () => {
  const navigator = useNavigate();

  const { message } = App.useApp();
  const onFinish = async (values: any) => {
    let res = await apiClient
      .request<any>(
        {
          url: '/user/login',
          method: 'POST',
          data: values,
        },
        {
          showError: false,
        },
      )
      .catch((res) => {
        console.log(res);
      });
    res = {
      userId: 1,
      nickName: 'admin',
    };
    userStore.setUserId(res.userId);
    userStore.setNickName(res.nickName);
    localStorage.setItem('isLogin', '1');
    navigator('/');
  };

  const onFinishFailed = (errorInfo: any) => {
    console.log('Failed:', errorInfo);
  };

  const register = () => {
    message.info('注册功能暂未开放');
  };

  return (
    <div className={styles.loginBox}>
      <div className={styles.loginCard}>
        <div className={styles.loginTitle}>
          <div>LZH</div>
          <div>React+Antd模板</div>
        </div>
        <div className={styles.loginSubtitle}>小小的项目描述</div>
        <Form
          labelCol={{ span: 8 }}
          wrapperCol={{ span: 16 }}
          className={styles.loginForm}
          onFinish={onFinish}
          onFinishFailed={onFinishFailed}
          autoComplete="off"
          size={'large'}>
          <Form.Item<LoginType>
            name="username"
            rules={[
              { required: true, message: '请输入用户名!' },
              { min: 4, message: '用户名最少4位!' },
            ]}>
            <Input prefix={<UserOutlined />} />
          </Form.Item>

          <Form.Item<LoginType>
            name="password"
            rules={[{ required: true, message: '请输入密码!' }]}>
            <Input.Password prefix={<LockOutlined />} autoComplete={'off'} />
          </Form.Item>

          <Form.Item wrapperCol={{ offset: 8, span: 20 }}>
            <Button type="primary" htmlType="submit">
              登录
            </Button>
            <Button type="primary" onClick={() => register()}>
              注册
            </Button>
          </Form.Item>
        </Form>
      </div>
    </div>
  );
};

export default LoginPage;
