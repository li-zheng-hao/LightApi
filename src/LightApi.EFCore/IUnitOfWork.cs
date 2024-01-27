namespace LightApi.EFCore;

public interface IUnitOfWork : IDisposable
{
    TransactionStatus Status { get; set; }

    void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool distributed = false);

    void Rollback();

    void Commit();

    Task RollbackAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);
}

public enum TransactionStatus
{
    /// <summary>
    /// 未开启
    /// </summary>
    UnOpened,
    /// <summary>
    /// 已开启
    /// </summary>
    Opened,
    /// <summary>
    /// 已提交
    /// </summary>
    Committed,
    /// <summary>
    /// 已回滚
    /// </summary>
    RolledBack
}