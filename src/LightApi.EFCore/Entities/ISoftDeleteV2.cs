namespace LightApi.EFCore.Entities;

public interface ISoftDeleteV2
{
    public bool? IsDeleted { get; set; }
    
    public DateTime? DeletedAt { get; set; }
    
    public void Undo()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}