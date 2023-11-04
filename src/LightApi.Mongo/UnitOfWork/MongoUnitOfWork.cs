#if NET6_0_OR_GREATER
using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Entities;

namespace LightApi.Mongo.UnitOfWork;

public class MongoUnitOfWork : IMongoUnitOfWork
{
    public void Dispose()
    {
        if (!IsCommited && !IsRollback&&MongoTransaction!=null)
        {
            CommitAsync().GetAwaiter().GetResult();
        }
        CapTransaction?.Dispose();
        MongoTransaction?.Dispose();
    }

    public Guid ContextId { get; set; } = new Guid();

    public ICapTransaction? CapTransaction { get; set; }

    public Transaction? MongoTransaction { get; set; }


    public void StartTransaction(IServiceProvider serviceProvider, bool useCapTransaction = false)
    {
        if (MongoTransaction != null) return;

        if (useCapTransaction)
        {
            var trans = DB.Transaction();
            var publisher = serviceProvider.GetRequiredService<ICapPublisher>();

            publisher.Transaction.Value =
                ActivatorUtilities.CreateInstance<MongoDBCapTransaction>(publisher.ServiceProvider);

            var capTrans = publisher.Transaction.Value.Begin(trans.Session, false);
            MongoTransaction = trans;
            CapTransaction = capTrans;
        }
        else
        {
            var trans = DB.Transaction();
            MongoTransaction = trans;
        }
    }

    private bool IsRollback { get; set; }

    private bool IsCommited { get; set; }
    public Task CommitAsync()
    {
        IsCommited = true;
        
        if (CapTransaction != null)
            return CapTransaction.CommitAsync();
        
        return MongoTransaction?.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (IsRollback)
            return;

        if (CapTransaction != null)
            await CapTransaction.RollbackAsync();
        else
            await MongoTransaction!.AbortAsync();

        IsRollback = true;
    }
}
#endif