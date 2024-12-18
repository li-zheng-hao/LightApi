namespace LightApi.EFCore.EFCore.Internal;

internal static class EFCoreUtil
{
    internal static object[] GetEntityKeyValues<TEntity>(
        Func<TEntity, object>[] keyValueGetter,
        TEntity entity
    ) => keyValueGetter.Select(x => x.Invoke(entity)).ToArray();
}
