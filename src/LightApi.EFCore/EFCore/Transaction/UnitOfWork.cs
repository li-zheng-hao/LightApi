namespace LightApi.EFCore.EFCore.Transaction;

public abstract class UnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    protected TDbContext AppDbContext { get; init; }
    protected IDbContextTransaction? DbTransaction { get; set; }
    public TransactionStatus Status { get; set; }

    protected UnitOfWork(TDbContext context)
    {
        AppDbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected abstract IDbContextTransaction GetDbContextTransaction(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        bool distributed = false
    );

    public virtual void BeginTransaction(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        bool distributed = false
    )
    {
        if (AppDbContext.Database.CurrentTransaction is not null)
            throw new ArgumentException(
                $"工作单元已开启,事务ID：{AppDbContext.Database.CurrentTransaction.TransactionId}"
            );

        DbTransaction = GetDbContextTransaction(isolationLevel, distributed);

        Status = TransactionStatus.Opened;
    }

    public virtual void Commit()
    {
        if (DbTransaction is null)
            throw new ArgumentNullException(nameof(DbTransaction), "IDbContextTransaction is null");

        if (Status == TransactionStatus.Opened)
        {
            DbTransaction.Commit();
            Status = TransactionStatus.Committed;
        }
    }

    public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (DbTransaction is null)
            throw new ArgumentNullException(nameof(DbTransaction), "IDbContextTransaction is null");
        if (Status == TransactionStatus.Opened)
        {
            await DbTransaction.CommitAsync(cancellationToken);
            Status = TransactionStatus.Committed;
        }
    }

    public virtual void Rollback()
    {
        if (DbTransaction is null)
            throw new ArgumentNullException(nameof(DbTransaction), "IDbContextTransaction is null");
        if (Status == TransactionStatus.Opened)
        {
            DbTransaction.Rollback();
            Status = TransactionStatus.RolledBack;
        }
    }

    public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (DbTransaction is null)
            throw new ArgumentNullException(nameof(DbTransaction), "IDbContextTransaction is null");
        if (Status == TransactionStatus.Opened)
        {
            await DbTransaction.RollbackAsync(cancellationToken);
            Status = TransactionStatus.RolledBack;
        }
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
