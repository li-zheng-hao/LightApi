using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightApi.EFCore.Entities;

public interface ISoftDeleteV3
{
    public DateTime? DeletedAt { get; set; }
    
    public void Undo()
    {
        DeletedAt = null;
    }

    public void Delete()
    {
        DeletedAt=DateTime.Now;
    }
  
}