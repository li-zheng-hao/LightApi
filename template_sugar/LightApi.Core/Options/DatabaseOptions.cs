namespace LightApi.Core.Options;

/// <summary>
/// 数据库相关配置
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    /// section名称
    /// </summary>
    public const string SectionName = "Databases";
    
    public MysqlStorage? Mysql{ get; set; }
    
    public SqlServerStorage? SqlServer { get; set; }
    
    public MongoDbStorage? MongoDb { get; set; }
    
    public RedisDbStorage? Redis { get; set; }
}

public class MysqlStorage
{
    public string ConnectionString { get; set; }
}

public class SqlServerStorage
{
    public string ConnectionString { get; set; }
}

public class MongoDbStorage
{
    public string ConnectionString { get; set; }
    
    public string DatabaseName { get; set; }
}

public class RedisDbStorage
{
    public string ConnectionString { get; set; }
}

