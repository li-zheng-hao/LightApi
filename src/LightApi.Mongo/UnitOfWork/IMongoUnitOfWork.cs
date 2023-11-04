#if NET6_0_OR_GREATER

using DotNetCore.CAP;
using MongoDB.Entities;

namespace LightApi.Mongo.UnitOfWork;

public interface IMongoUnitOfWork : IDisposable
{
    /// <summary>
    /// 上下文ID
    /// </summary>
    Guid ContextId { get; set; }
    
    ICapTransaction CapTransaction { get; set; }

    Transaction MongoTransaction { get; set; }

    /// <summary>
    /// 开启事务
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="useCapTransaction">是否把CAP加入事务</param>
    void StartTransaction(IServiceProvider serviceProvider,bool useCapTransaction = false);
    
    Task CommitAsync();

    Task RollbackAsync();
    

}

#endif
