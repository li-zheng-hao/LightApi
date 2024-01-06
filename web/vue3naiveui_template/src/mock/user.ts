import { type MockMethod } from 'vite-plugin-mock';
// 单纯的使⽤⾃⼰的返回数据话，可以不⽤引⼊mockjs
// http://mockjs.com/examples.html
import * as Mock from 'mockjs';
export default [
    {
        url: '/api/user',
        method: 'get',
        response: () => {
            return {
                success:Mock.Random.boolean(),
                code: 200,
                msg: '接口返回的消息',
                data: Mock.mock({
                    'list|20': [
                        {
                            id: '@id',
                            name: '@cname',
                            age: Mock.Random.integer(1, 100),
                            address: '@county',
                            city: '@city',
                            province: '@province',
                            email: Mock.Random.email(),
                            phone: /^1[0-9]{10}$/,
                            regin: '@region',
                            url: '@url',
                            date: Mock.Random.date('yyyy-MM-dd')
                        }
                    ]
                }).list
            };
        }
    }
] as MockMethod[];
