import { _ } from './common';

/**
 * 根据路由路径查找路由信息
 * @param routePath
 * @param routes
 */
export const findRoute = (routePath: string, routes: any): any => {
  for (let i = 0; i < routes.length; i++) {
    const item = routes[i];
    if (item.path == routePath) {
      return item;
    }
    if (!_.isEmpty(item.children)) {
      const info = findRoute(routePath, item.children);
      if (info) {
        return info;
      }
    }
  }
};
