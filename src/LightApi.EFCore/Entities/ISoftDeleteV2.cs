using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightApi.EFCore.Entities;

/// <summary>
/// 软删除标记 建议用V3版本
/// </summary>
[Obsolete("使用ISoftDeleteV3")]
public interface ISoftDeleteV2
{
    public bool? IsDeleted { get; set; }

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
