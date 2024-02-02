using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.EFCore.Transaction;

namespace LightApi.EFCore.Sqlite.Transaction;

public class SqliteUnitOfWork<TDbContext> : UnitOfWork<TDbContext>
    where TDbContext : AppDbContext
{
    private ICapPublisher? _publisher;

    public SqliteUnitOfWork(
        TDbContext context
        , ICapPublisher? publisher = null)
        : base(context)
    {
        _publisher = publisher;
    }

    protected override IDbContextTransaction GetDbContextTransaction(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
        , bool distributed = false)
    {
        if (distributed)
            if (_publisher is null)
                throw new ArgumentException("CapPublisher is null");
            else
                return AppDbContext.Database.BeginTransaction(_publisher, false);
        else
            return AppDbContext.Database.BeginTransaction(isolationLevel);
    }
}