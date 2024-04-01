# EFCore框架发布记录

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
