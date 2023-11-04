using System;
using System.Threading;
using System.Threading.Tasks;

namespace FB.Infrastructure.EFCore.Repository.IRepositories;

public interface IUnitOfWork : IDisposable
{
    bool IsStartingUow { get; }

    void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool distributed = false);

    void Rollback();

    void Commit();

    Task RollbackAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);
}