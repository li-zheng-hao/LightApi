using LightApi.EFCore.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LightApi.EFCore.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is null) return base.SavingChanges(eventData, result);

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is { State: EntityState.Deleted, Entity: ISoftDelete delete })
            {
                entry.State = EntityState.Modified;
                delete.IsDeleted = true;
                delete.DeletedAt = DateTime.Now;
            }
            else if (entry is { State: EntityState.Deleted, Entity: ISoftDeleteV2 delete2 })
            {
                entry.State = EntityState.Modified;
                delete2.IsDeleted = true;
                delete2.DeletedAt = DateTime.Now;
            }
            else if (entry is { State: EntityState.Deleted, Entity: ISoftDeleteV3 delete3 })
            {
                entry.State = EntityState.Modified;
                delete3.DeletedAt = DateTime.Now;
            };
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is { State: EntityState.Deleted, Entity: ISoftDelete delete })
            {
                entry.State = EntityState.Modified;
                delete.IsDeleted = true;
                delete.DeletedAt = DateTime.Now;
            }
            else if (entry is { State: EntityState.Deleted, Entity: ISoftDeleteV2 delete2 })
            {
                entry.State = EntityState.Modified;
                delete2.IsDeleted = true;
                delete2.DeletedAt = DateTime.Now;
            }
           
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}