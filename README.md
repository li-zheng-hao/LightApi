# LightApi

AspNetCore Webapi项目框架及模板，包含几个模块：

1. LightApi.Infra 基础设施模块，包含一些基础设施功能，如：日志、缓存、消息队列、工具类等
2. LightApi.EFCore EFCore模块，包含EFCore相关类库，如：DbContext、Repository、UnitOfWork等
3. LightApi.Mongo MongoDB模块，包含MongoDB相关类库，如：Repository、UnitOfWork等
4. LightApi.SqlSugar SqlSugar模块，包含SqlSugar相关类库，如：Repository、UnitOfWork等

# 项目模板

项目模板位于项目根目录下的template目录，包含以下几个模板：

1. `template_sugar` SqlSugar作为ORM的Webapi开发模板
2. `template_efcore` EFCore作为ORM的Webapi开发模板

# Nuget包

| 包名                | 地址                                               | 
|-------------------|--------------------------------------------------| 
| LightApi          | https://www.nuget.org/packages/LightApi.Infra    |
| LightApi.EFCore   | https://www.nuget.org/packages/LightApi.EFCore   |
| LightApi.Mongo    | https://www.nuget.org/packages/LightApi.Mongo    |
| LightApi.SqlSugar | https://www.nuget.org/packages/LightApi.SqlSugar |

# 更新日志

1. [Infra模块发布记录](RELEASE_Infra.md)
2. [EFCore模块发布记录](RELEASE_EFCore.md)
3. [MongoDB模块发布记录](RELEASE_Mongo.md)
4. [SqlSugar模块发布记录](RELEASE_SqlSugar.md)
