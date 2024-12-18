namespace LightApi.EFCore.Entities;

public class EfEntity<T> : IAuditable, IEfEntity<T>
{
    public T Id { get; set; }

    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }
}
