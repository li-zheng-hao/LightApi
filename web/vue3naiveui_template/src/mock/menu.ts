import { type MockMethod } from 'vite-plugin-mock';
export default [
    {
        url: '/api/menu',
        method: 'get',
        response: () => {
            return {
                success:true,
                code: 200,
                msg: 'ok',
                data: [
                    {
                        id:'1',
                        title:'首页',
                    },
                    {
                        id:'2',
                        title:'用户管理',
                    },

                ]
            };
        }
    }
] as MockMethod[];
