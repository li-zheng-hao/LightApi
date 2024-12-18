namespace LightApi.EFCore.Entities;

public interface IEntityInfo
{
    void OnModelCreating(dynamic modelBuilder);
}
