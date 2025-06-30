using LightApi.EFCore.Entities;
using LightApi.EFCore.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.EFCore.EFCore.DbContext;

public partial class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly IServiceProvider? _serviceProvider;
    protected IEntityInfo? _entityInfo;

    /// <summary>
    ///
    /// </summary>
    /// <param name="options"></param>
    /// <param name="serviceProvider">外部服务提供者</param>
    public AppDbContext(DbContextOptions options, IServiceProvider serviceProvider)
        : base(options)
    {
        _serviceProvider = serviceProvider;
        // Database.AutoTransactionsEnabled = false;
        // ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        // Database.AutoTransactionBehavior = AutoTransactionBehavior.WhenNeeded;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();

        var result = base.SaveChangesAsync(cancellationToken);

        return result;
    }

    public override int SaveChanges()
    {
        SetAuditFields();

        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Type entityInfoType = Db.GetEntityInfoType(this);

        _entityInfo ??= _serviceProvider!.GetService(entityInfoType) as IEntityInfo;

        _entityInfo!.OnModelCreating(modelBuilder);
    }

    protected virtual int SetAuditFields()
    {
        // var operater = _entityInfo.GetOperater();
        var allBasicAuditEntities = ChangeTracker
            .Entries<IAuditable>()
            .Where(x => x.State == EntityState.Added);
        allBasicAuditEntities.ForEach(entry =>
        {
            // entry.Entity.CreateBy = operater.Id;
            entry.Entity.CreateTime = DateTime.Now;
        });

        var auditFullEntities = ChangeTracker
            .Entries<IAuditable>()
            .Where(x => x.State == EntityState.Modified || x.State == EntityState.Added);
        auditFullEntities.ForEach(entry =>
        {
            // entry.Entity.ModifyBy = operater.Id;
            entry.Entity.UpdateTime = DateTime.Now;
        });

        return ChangeTracker.Entries<IEfEntity>().Count();
    }
}
