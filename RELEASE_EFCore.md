# EFCore模块更新日志

## 2025年7月1日 3.0.0

1. 删除仓储代码，直接使用DbContext

## 2025年6月27日  2.0.0

1. 仓储类Dapper相关接口调整

## 2024年10月15日  1.2.1

1. 软删除v3版本删除IsDeleted字段，仅通过时间戳判断

## 2024年9月15日  1.2.0

1. 拆分分页查询部分到LightApi.Common中（原本在EFCore模块中）

## 2024年4月16日 EFCore、MySql、SqlServer扩展 1.1.0-preview

1. .NET7、.NET8多目标输出,在.NET8下使用EFCore 8
2. 升级Dapper版本
3. 新增Dapper.SqlBuilder依赖，新增SqlBuilder WhereIf扩展

## 2024年4月2日 EFCore 1.0.1

1. SaveChanges返回值修改为Task<int>

## 2024年2月2日 EFCore

1. 新增Sqlite扩展

## 2024年1月27日 EFCore、MySql、SqlServer扩展 1.0.0 （不兼容更新）

1. 支持多库

## 2024年1月6日 MySql+SqlServer扩展 0.2.0

1. ServiceExtension新增参数IServiceProvider

## 2024年1月6日 0.4.0

1. ID字段在System.Text.Json和Newtonsoft.Json序列化时，排在第一位（优先序号为-10）

## 2023年11月16日 0.3.0

1. 仓储新增根据id删除接口

## 2023年11月16日 0.2.0

1. 软删除不需要再继承AbstractEntityTypeConfiguration类

## 2023年11月16日 0.1.2

1. 修复GetById查询问题
