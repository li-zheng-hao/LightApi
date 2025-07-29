# Infra模块更新日志

## 2025年7月29日 4.2.1

1. RabbitMqManager允许返回null


## 2025年7月21日 4.2.0

1. FileStorage增加删除mongodb文件接口
2. 优化FileStorage下载MongoDB文件key参数，支持读取url
3. 修复自定义CookieAuth注销登录错误问题

## 2025年7月1日 4.1.4

1. 优化启动速度

## 2025年5月20日 4.1.3

1. 升级CliWrap
2. 调整ProcessInvokeHelper参数和返回值读取逻辑

## 2025年5月20日 4.1.2

1. 修复rabbitmqmanager未初始化连接的问题

## 2025年4月18日 4.1.1

1. minio依赖升级，当上传minio失败时抛出异常

## 2025年4月9日 4.1.0

1. minio支持生成预签名url

## 2025年3月19日 4.0.1

1. 如果模型验证错误只有一条，则不会再放到data中返回
2. 框架选项新增ReturnModelValidateErrorMessageInExtraInfo参数，默认false，如果存在多条模型验证错误时，将其返回到extraInfo部分。

## 2024年12月18日 4.0.0-preview1

不兼容更新：

1. Rougamo.Fody升级2.0.0=>5.0.0
2. 升级RabbitMQ.Client升级到7.0.0
3. 删除OpLogAttribute
4. MongoDB.Entities升级到24.x，Mongo官方驱动从2.x到3.x版本
5. Masuit库由Masuit.Tools.AspNetCore改为使用Masuit.Tools.Abstrations

其它更新

1. 升级其它依赖

## 2024年11月28日 3.9.1

1. 修复DynamicOrder忽略大小写问题

## 2024年11月18日 3.9.0

1. ProcessInvokeHelper新增无返回结果调用方式

## 2024年10月08日 3.8.3

1. ProcessInvokeHelper序列化配置支持自定义

## 2024年9月15日 3.8.1,3.8.2

1. DynamicWhere首字母转大写

## 2024年9月15日 3.8.0

1. 拆分分页查询部分到LightApi.Common中（原本在EFCore模块中）

## 2024年9月12日 3.7.0

1. 新增MongoDB存储

## 2024年8月22日 3.6.0

1. 新增DynamicOrderIfOrDefault

## 2024年7月16日 3.5.2

1. 参数校验优化

## 2024年7月9日 3.5.1

1. LogAction支持Opentelemetry

## 2024年7月6日 3.5.0

1. 新增动态查询扩展 `DynamicWhere`
2. 升级Serilog依赖版本

## 2024年7月1日 3.4.3

1. 修复ExcelHelper.GetValuesByNpoi读取空单元格异常的问题

## 2024年7月1日 3.4.2

1. 修复ExcelHelper.GetValuesByNpoi最后一行没有读取的问题

## 2024年6月28日 3.4.1

1. 修复IUser和IUser实现类获取实例时不是同一个对象的问题

## 2024年6月22日 3.4.0

1. 优化ExcelHelper,新增NPOI依赖，新增方法
2. 修复问题：ExcelHelper在导入数据时将所有空行都删除(升级可能对现有业务造成影响)
3. 数据校验类在对空对象时改为返回成功，之前是失败
4. Check新增方法

## 2024年6月12日 3.3.0

1. 文件存储功能

## 2024年6月12日 3.2.2

1. ExcelHelper校验

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