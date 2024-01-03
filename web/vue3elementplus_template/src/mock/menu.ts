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
                            icon: 'UserOutlined',
                            routePath:'/'
                        },
                        {
                            label: '系统设置',
                            key: '/sys',
                            icon: 'UserOutlined',
                            routePath: null,
                            children: [
                                {
                                    label: '角色权限管理',
                                    key: '/sys/role',
                                    icon: 'UserOutlined',
                                    routePath:'/sys/role'
                                },
                                {
                                    label: '菜单权限管理',
                                    key: '/sys/menu',
                                    icon: 'UserOutlined',
                                    routePath:'/sys/menu'
                                },
                            ]

                        },
                        {
                            label: '异常页面',
                            key: '/error',
                            icon: 'UserOutlined',
                            routePath:null,
                            children: [
                                {
                                    label: '404',
                                    key: '/error/404',
                                    icon: 'UserOutlined',
                                    routePath:'/error/404'

                                },
                                {
                                    label: '403',
                                    key: '/error/403',
                                    icon: 'UserOutlined',
                                    routePath:'/error/403'

                                },
                                {
                                    label: '500',
                                    key: '/error/500',
                                    icon: 'UserOutlined',
                                    routePath:'/error/500'

                                },
                            ]
                        },
                        {
                            label: '结果页面',
                            key: '/result',
                            icon: 'UserOutlined',
                            routePath:null,
                            children: [
                                {
                                    label: '成功',
                                    key: '/result/success',
                                    icon: 'UserOutlined',
                                    routePath:'/result/success'

                                },
                                {
                                    label: '失败',
                                    key: '/result/fail',
                                    icon: 'UserOutlined',
                                    routePath:'/result/fail'

                                }, {
                                    label: '信息',
                                    key: '/result/info',
                                    icon: 'UserOutlined',
                                    routePath:'/result/info'
                                }
                            ]
                        }
                    ]
            };
        }
    }
] as MockMethod[];



