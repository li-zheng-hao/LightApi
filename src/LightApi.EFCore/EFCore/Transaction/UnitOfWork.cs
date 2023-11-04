using FB.Infrastructure.EFCore.Repository.IRepositories;

namespace LightApi.EFCore.EFCore.Transaction;

public abstract class UnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    protected TDbContext AppDbContext { get; init; }
    protected IDbContextTransaction? DbTransaction { get; set; }

    public bool IsStartingUow => AppDbContext.Database.CurrentTransaction is not null;

    protected UnitOfWork(TDbContext context)
    {
        AppDbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected abstract IDbContextTransaction GetDbContextTransaction(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool distributed = false);

    public virtual void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        bool distributed = false)
    {
        if (AppDbContext.Database.CurrentTransaction is not null)
            throw new ArgumentException($"UnitOfWork Error,{AppDbContext.Database.CurrentTransaction}");
        else
            DbTransaction = GetDbContextTransaction(isolationLevel, distributed);
    }

    public virtual void Commit()
    {
        if (DbTransaction is null)
            throw new ArgumentNullException(nameof(DbTransaction), "IDbContextTransaction is null");
        else
            DbTransaction.Commit();
    }

    public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (DbTransaction is null)
            throw new ArgumentNullException(nameof(DbTransaction), "IDbContextTransaction is null");
        else
            await DbTransaction.CommitAsync(cancellationToken);
    }

    public virtual void Rollback()
    {
        if (DbTransaction is null)
            throw new ArgumentNullException(nameof(DbTransaction), "IDbContextTransaction is null");
        else
            DbTransaction.Rollback();
    }

    public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (DbTransaction is null)
            throw new ArgumentNullException(nameof(DbTransaction), "IDbContextTransaction is null");
        else
            await DbTransaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (DbTransaction is not null)
            {
                DbTransaction.Dispose();
                DbTransaction = null;
            }
        }
    }
}