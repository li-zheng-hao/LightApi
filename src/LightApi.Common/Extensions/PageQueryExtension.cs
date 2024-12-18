using LightApi.Common.Page;

namespace LightApi.Common.Extensions;

public static class PageQueryExtension
{
    /// <summary>
    /// 分页拓展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entities"></param>
    /// <param name="pageIndex">页码，必须大于0</param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static PageList<TEntity> ToPageList<TEntity>(
        this IQueryable<TEntity> entities,
        int pageIndex = 1,
        int pageSize = 20
    )
    {
        if (pageIndex <= 0)
            throw new InvalidOperationException(
                $"{nameof(pageIndex)} must be a positive integer greater than 0."
            );

        var totalCount = entities.Count();
        var items = entities.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        return new PageList<TEntity>(items, pageIndex, pageSize, totalCount);
    }
}
