import { useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { routes, RouteInfo } from ".";
import nprogress from "../utils/nprogress";

const getCurrentRouterMap = (routers: RouteInfo[], path: string): RouteInfo => {
  for (const router of routers) {
    if (router.path == path) return router;
    if (router.children) {
      const childRouter = getCurrentRouterMap(router.children, path);
      if (childRouter) return childRouter;
    }
  }
  return routes[routes.length - 1];
};

// 路由守卫 在这里处理
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const RouteGuard = ({ children }: any) => {
  const location = useLocation();
  const navigator = useNavigate();
  useEffect(() => {
    nprogress.done();
    return () => nprogress.start();
  });
  useEffect(() => {
    // TODO 权限校验或其它前置校验    
    // const router = getCurrentRouterMap(routes, location.pathname);
    // if (router.isAuth) {
      // navigator("/login");
    // }
  }, [location.pathname, navigator]);
  return children;
};
