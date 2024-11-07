import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import nprogress from '../utils/nprogress';
import { App } from 'antd';

// const getCurrentRouterMap = (routers: RouteInfo[], path: string): RouteInfo => {
//   for (const router of routers) {
//     if (router.path == path) return router;
//     if (router.children) {
//       const childRouter = getCurrentRouterMap(router.children, path);
//       if (childRouter) return childRouter;
//     }
//   }
//   return routes[routes.length - 1];
// };

// 路由守卫 在这里处理
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const RouteGuard = ({ children }: any) => {
  const { message } = App.useApp();
  const navigator = useNavigate();
  const isLogin = localStorage.getItem('isLogin');

  useEffect(() => {
    if (!isLogin) {
      message.info('请先登录');
      navigator('/login');
    }
  }, [isLogin, navigator, message]);

  if (isLogin) {
    return children;
  } else {
    return undefined;
  }
};
