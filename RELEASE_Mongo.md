# Mongo模块更新日志

## 2025年4月9日 4.0.0

不兼容更新：

1. Mongo.Driver从2.x升级到3.x版本

## 2025年3月7日 3.1.3

1. 修复工作单元特性问题

## 2024年9月15日 3.1.1,3.1.2

1. 修复工作单元无效问题

## 2024年9月15日 3.1.0

1. 拆分分页查询部分到LightApi.Common中（原本在EFCore模块中）

## 2024年9月12日 3.0.0

1. Mongo依赖注入Scoped的DbContext,工作单元调整删除DbTransaction

## 2024年9月10日 2.0.0

1. Mongo.Entities升级到23.1.1版本，不支持framework
2. 删除仓储

