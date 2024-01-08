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
                success:true,
                code: 200,
                msg: '',
                data: Mock.mock({
                    'list|100': [
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
    },
    {
        url:'/api/user/login',
        method:'post',
        response:({body})=>{
            const {username,password} = body;
            if(username === 'admin' && password === '123456'){
                return {
                    success:true,
                    code:200,
                    msg:'登录成功',
                    data:{
                        username:'李正浩',
                        avator:'LZH',
                        permissions:['admin'],
                        accessToken:'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiYWRtaW4iLCJpZCI6MSwiZXhwIjoxNjM5MjM4NDAwfQ.bDbZ2LHdSxNEYHyj8TqIBohnjjKl5HA3LiaASQsFuc8',
                        refreshToken:'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiYWRtaW4iLCJpZCI6MSwiZXhwIjoxNzMzOTMyODAwfQ.B3P01o9F7hCIPxsdF_dmrB-2r9fuAtABKyd7deIfPz0'
                    }
                }
            }else{
                return {
                    success:false,
                    code:400,
                    msg:'登录失败,账号密码错误',
                    data:null
                }
            }
        }
    },
    {
        url:'/api/user/refresh-token',
        method:'post',
        async rawResponse(req, res) {
            let reqbody = ''
            await new Promise((resolve) => {
                req.on('data', (chunk) => {
                    reqbody += chunk
                })
                req.on('end', () => resolve(undefined))
            })
            const {token}=JSON.parse(reqbody)
            if (token === 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiYWRtaW4iLCJpZCI6MSwiZXhwIjoxNzMzOTMyODAwfQ.B3P01o9F7hCIPxsdF_dmrB-2r9fuAtABKyd7deIfPz0') {
                res.writeHead(200, {
                    'Content-Type': 'application/json; charset=utf-8',
                });
                res.end(
                    JSON.stringify({
                        success: true,
                        code: 200,
                        msg: '刷新成功',
                        data: {
                            accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiYWRtaW4iLCJpZCI6MSwiZXhwIjoxNzMzOTMyODAwfQ.B3P01o9F7hCIPxsdF_dmrB-2r9fuAtABKyd7deIfPz0',
                            refreshToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiYWRtaW4iLCJpZCI6MSwiZXhwIjoxNzMzOTMyODAwfQ.B3P01o9F7hCIPxsdF_dmrB-2r9fuAtABKyd7deIfPz0',
                        },
                    })
                );
            } else {
                res.writeHead(401, {
                    'Content-Type': 'application/json; charset=utf-8',
                });
                res.end(
                    JSON.stringify({
                        success: false,
                        code: 401,
                        msg: '刷新失败',
                        data: null
                    })
                );
            }
        },
    },
    {
        url:'/api/user/info',
        method:'get',
        response:()=>{
            return {
                success:true,
                code:200,
                msg:'',
                data:{
                    username:'李正浩',
                    avator:'LZH',
                    permissions:['admin'],
                }
            }
        }
    }
] as MockMethod[];
