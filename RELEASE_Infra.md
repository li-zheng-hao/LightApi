# Infra框架发布记录

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