import { type MockMethod } from 'vite-plugin-mock';
// 单纯的使⽤⾃⼰的返回数据话，可以不⽤引⼊mockjs
// http://mockjs.com/examples.html
import Mock, { Random } from 'mockjs';
export default [
    {
        url: '/api/user',
        method: 'get',
        response: () => {
            return {
                success:true,
                code: 200,
                msg: 'ok',
                data: Mock.mock({
                    'list|4': [
                        {
                            id: '@id',
                            name: '@cname',
                            age: Random.integer(1, 100),
                            address: '@county',
                            city: '@city',
                            province: '@province',
                            email: Random.email(),
                            phone: /^1[0-9]{10}$/,
                            regin: '@region',
                            url: '@url',
                            date: Random.date('yyyy-MM-dd')
                        }
                    ]
                })
            };
        }
    }
] as MockMethod[];
