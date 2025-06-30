using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightApi.EFCore.Entities;

/// <summary>
/// 软删除标记 需要在表第一次建立的时候加，如果是后加上去的用V3版本
/// </summary>
[Obsolete("使用ISoftDeleteV3")]
public interface ISoftDelete
{
    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public void Undo()
    {
        IsDeleted = false;
        DeletedAt = null;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.Now;
    }
}
