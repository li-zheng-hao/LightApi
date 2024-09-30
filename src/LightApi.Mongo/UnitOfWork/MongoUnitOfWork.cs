using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;

namespace LightApi.Mongo.UnitOfWork;

public class MongoUnitOfWork : IMongoUnitOfWork
{
    public void Dispose()
    {
        if (!IsCommited && !IsRollback&&IClientSessionHandle!=null)
        {
            CommitAsync().GetAwaiter().GetResult();
        }
        CapTransaction?.Dispose();
        IClientSessionHandle?.Dispose();
    }

    public Guid ContextId { get; set; } = new Guid();

    public ICapTransaction? CapTransaction { get; set; }

    public IClientSessionHandle? IClientSessionHandle { get; set; }
    
    public DBContext DbContext { get; set; }


    public void StartTransaction(IServiceProvider serviceProvider, bool useCapTransaction = false)
    {
        if (IClientSessionHandle != null) return;

        DbContext=serviceProvider.GetRequiredService<DBContext>();
        
        if (useCapTransaction)
        {
            var trans = DbContext.Transaction();
            var publisher = serviceProvider.GetRequiredService<ICapPublisher>();
            publisher.Transaction =
            ActivatorUtilities.CreateInstance<MongoDBCapTransaction>(publisher.ServiceProvider);
            publisher.Transaction.DbTransaction=  trans;
            publisher.Transaction.AutoCommit = false;
            IClientSessionHandle = trans;
            CapTransaction = publisher.Transaction;
        }
        else
        {
            var trans = DbContext.Transaction();
            IClientSessionHandle = trans;
        }
    }

    private bool IsRollback { get; set; }

    private bool IsCommited { get; set; }
    public Task CommitAsync()
    {
        IsCommited = true;
        
        if (CapTransaction != null)
            return CapTransaction.CommitAsync();
        
        return IClientSessionHandle!.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        if (IsRollback)
            return;

        if (CapTransaction != null)
            await CapTransaction.RollbackAsync();
        else
            await IClientSessionHandle!.AbortTransactionAsync();

        IsRollback = true;
    }

    public bool UseOptimisticLocker { get; set; }
}
