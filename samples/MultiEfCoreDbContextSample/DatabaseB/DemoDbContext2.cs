﻿using LightApi.EFCore.EFCore.DbContext;
using Microsoft.EntityFrameworkCore;

namespace MultiEfCoreDbContextSample.Database;

public class DemoDbContext2:AppDbContext
{
    public DemoDbContext2(DbContextOptions<DemoDbContext2> options,IServiceProvider serviceProvider) : base(options,serviceProvider)
    {
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        
        // convert enum to string
        configurationBuilder.Properties<Enum>()
            .HaveConversion<string>()
            .HaveMaxLength(256);
        
        // 默认string长度
        configurationBuilder.Properties<string>()
            //.AreUnicode(false)
            //.AreFixedLength()
            .HaveMaxLength(256);
        
        // 默认decimal精度
        configurationBuilder.Properties<decimal>()
            .HavePrecision(18, 2);

    }
}