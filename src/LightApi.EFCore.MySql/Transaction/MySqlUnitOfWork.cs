using DotNetCore.CAP;
using LightApi.EFCore.EFCore.DbContext;
using LightApi.EFCore.EFCore.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LightApi.EFCore.MySql.Transaction;

public class MySqlUnitOfWork<TDbContext> : UnitOfWork<TDbContext>
    where TDbContext : AppDbContext
{
    private ICapPublisher? _publisher;

    public MySqlUnitOfWork(TDbContext context, ICapPublisher? publisher = null)
        : base(context)
    {
        _publisher = publisher;
    }

    protected override IDbContextTransaction GetDbContextTransaction(
        System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted,
        bool distributed = false
    )
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
