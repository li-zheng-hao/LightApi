// See https://aka.ms/new-console-template for more information

using LightApi.Core;
using LightApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// 不同环境的数据库连接字符串
// const string connectionString = "Data Source=xxx.db";

IServiceCollection serviceCollection=new ServiceCollection();

serviceCollection.AddEfCoreSqliteSetup();

var sp=serviceCollection.BuildServiceProvider();

var context = sp.GetRequiredService<FbAppContext>();

context.Database.Migrate();

Console.WriteLine("finished");

