namespace LightApi.EFCore.Entities;

public interface IAuditable
{
    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }
}
