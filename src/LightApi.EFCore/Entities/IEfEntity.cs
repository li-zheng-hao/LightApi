namespace LightApi.EFCore.Entities;

public interface IEfEntity
{
    
}

public interface IEfEntity<T> : IEfEntity 
{
    public T Id { get; set; }
}