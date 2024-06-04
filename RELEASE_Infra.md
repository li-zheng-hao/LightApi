# Infra框架发布记录

## 2024年6月4日 3.2.1

1. Cookie配置调整

## 2024年6月3日 3.2.0

1. Cookie认证扩展

## 2024年5月30日 3.1.0

1. Redis分布式锁

## 2024年4月18日 3.0.0

中断修改：

1. 服务注册方式调整，统一使用IServiceCollection扩展方法，而不是全都在AddInfrastructure
2. IUser的Id调整为string类型

新增功能：

1. 新增Swagger文件上传过滤器、过期接口过滤器、枚举类型注释生成过滤器

## 2024年4月18日 2.0.4

1. 修复ProcessInvokeHelper类Bug,新增调用参数
2. 调整Check工具类
3. 调整UseUserContext注册函数

## 2024年4月8日 2.0.3

1. 修复参数校验默认错误消息多了个'$'字符的问题

## 2024年3月21日 2.0.2

1. 调整RabbitMq部分类的命名空间

## 2024年3月21日 2.0.1

1. ExcelHelper修改

## 2024年2月27日 2.0.0

1. RabbitMq ExchangeConfig调整，新增AutoCreate选项

## 2024年2月27日 2.0.0-preview2

1. RabbitMqManager调整，方便替换服务

## 2024年2月26日 2.0.0-preview1 (不兼容升级) 

1. RabbitMq调整,不使用EasyNetQ，直接用RabbitMQ.Client库

## 2024年1月25日 1.0.0 (不兼容升级)

1. 统一结果包装新增附加信息字段
2. 框架配置新增选项IncludeUnCatchExceptionTraceInfo、IncludeExceptionStack，用于配置是否返回错误信息
3. DynamicOrder首字母统一转大写
4. 删除旧的OrderExpressExtension类

## 2024年1月25日 0.10.0

1. ExcelHelper新增ThrowIfAny

## 2024年1月25日 0.9.0

1. Linq扩展，支持DynamicOrderIf，WhereIf

## 2024年1月18日 0.8.0

1. 新增ExcelHelper，用于校验和导入excel数据

## 2024年1月17日 0.7.2

1. 修复类型转换错误

## 2024年1月16日 0.7.1

1. 修复过滤器并发问题

## 2024年1月15日 0.7.0

1. 新增AddOperationIdAttribute,用于给日志上下文新增一个OperationId字段，方便日志查询过滤

## 2023年12月15日 0.6.0

1. ProcessInvokeHelper 新增TimeOut参数
2. 新增RabbitMqManager，对EasyNetQ进一步封装

## 2023年12月15日 0.5.2

1. 新增ProcessInvokeHelper工具类，用于进程调用

## 2023年12月4日 0.5.1

1. 新增MinValueEx特性
2. RangeEx支持decimal转换

## 2023年11月16日 0.5.0

1. 业务异常支持控制HTTP状态码

## 2023年11月16日 0.4.2

1. App类新增程序生命周期字段AppLifeTime

## 2023年11月16日 0.4.1

1. 修复HTTP请求体只能读取一次的BUG

## 2023年11月16日 0.4.0

1. 日志记录AOP添加用户名信息

## 2023年11月16日 0.3.1

1. 动态排序在未找到Key时抛出业务异常

## 2023年11月15日 0.3.0

1. 调整BusinessException,删除HttpCode属性
2. InfrastructureOptions新增参数，DefaultFailureBusinessExceptionCode用于控制默认业务错误码，
DefaultUnCatchedExceptionCode用于控制默认未捕获异常错误码，UseFirstModelValidateErrorMessage将第一个模型校验错误信息作为错误信息返回
3. 汉化aspnetcore框架默认模型验证错误

> 以上这条为示例