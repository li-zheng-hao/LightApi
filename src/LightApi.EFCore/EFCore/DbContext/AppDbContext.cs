using LightApi.EFCore.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LightApi.EFCore.EFCore.DbContext;

public partial class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    protected IEntityInfo? _entityInfo;
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
        // Database.AutoTransactionsEnabled = false;
        // ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        // Database.AutoTransactionBehavior = AutoTransactionBehavior.WhenNeeded;
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // var changedEntities = ChangeTracker.Entries<IEfEntity>().Count();
        //
        // //没有自动开启事务的情况下,保证主从表插入，主从表更新开启事务。
        // var isManualTransaction = false;
        // if (Database.AutoTransactionBehavior==AutoTransactionBehavior.Never && Database.CurrentTransaction is null && changedEntities > 1)
        // {
        //     isManualTransaction = true;
        //     Database.AutoTransactionBehavior = AutoTransactionBehavior.Always;
        // }

        SetAuditFields();
        
        var result = base.SaveChangesAsync(cancellationToken);
    
        // //如果手工开启了自动事务，用完后关闭。
        // if (isManualTransaction)
        //     Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    
        return result;
    }

    public override int SaveChanges()
    {
        SetAuditFields();
        
        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if(_entityInfo is null)
            _entityInfo = this.GetService<IEntityInfo>();
        _entityInfo.OnModelCreating(modelBuilder);
    }

    protected virtual int SetAuditFields()
    {
        // var operater = _entityInfo.GetOperater();
        var allBasicAuditEntities = ChangeTracker.Entries<IAuditable>().Where(x => x.State == EntityState.Added);
        allBasicAuditEntities.ForEach(entry =>
        {
            // entry.Entity.CreateBy = operater.Id;
            entry.Entity.CreateTime = DateTime.Now;
        });
        
        var auditFullEntities =ChangeTracker.Entries<IAuditable>().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added);
        auditFullEntities.ForEach(entry =>
        {
            // entry.Entity.ModifyBy = operater.Id;
            entry.Entity.UpdateTime = DateTime.Now;
        });
    
        return ChangeTracker.Entries<IEfEntity>().Count();
    }
}