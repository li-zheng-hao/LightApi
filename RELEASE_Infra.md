# Infra框架发布记录

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