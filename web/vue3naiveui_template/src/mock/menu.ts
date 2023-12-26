import {type MockMethod} from 'vite-plugin-mock';
import Mock, {Random} from "mockjs";

export default [
    {
        url: '/api/menu',
        method: 'get',
        response: () => {
            return {
                success: true,
                code: 200,
                msg: 'ok',
                data:
                    [
                        {
                            label: '首页',
                            key: '/',
                            icon: 'order',
                            routePath:'/'
                        },
                        {
                            label: '系统设置',
                            key: '/sys',
                            icon: 'order',
                            routePath: null,
                            children: [
                                {
                                    label: '角色权限管理',
                                    key: '/sys/role',
                                    icon: 'order',
                                    routePath:'/sys/role'
                                },
                                {
                                    label: '菜单权限管理',
                                    key: '/sys/menu',
                                    icon: 'order',
                                    routePath:'/sys/menu'
                                },
                            ]

                        },
                        {
                            label: '异常页面',
                            key: '/error',
                            icon: 'order',
                            routePath:null,
                            children: [
                                {
                                    label: '404',
                                    key: '/error/404',
                                    icon: 'order',
                                    routePath:'/error/404'

                                },
                                {
                                    label: '403',
                                    key: '/error/403',
                                    icon: 'order',
                                    routePath:'/error/403'

                                },
                                {
                                    label: '500',
                                    key: '/error/500',
                                    icon: 'order',
                                    routePath:'/error/500'

                                },
                            ]
                        },
                        {
                            label: '结果页面',
                            key: '/result',
                            icon: 'order',
                            routePath:null,
                            children: [
                                {
                                    label: '成功',
                                    key: '/result/success',
                                    icon: 'order',
                                    routePath:'/result/success'

                                },
                                {
                                    label: '失败',
                                    key: '/result/fail',
                                    icon: 'order',
                                    routePath:'/result/fail'

                                }, {
                                    label: '信息',
                                    key: '/result/info',
                                    icon: 'order',
                                    routePath:'/result/info'
                                }
                            ]
                        }
                    ]
            };
        }
    }
] as MockMethod[];



