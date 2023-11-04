using System.ComponentModel.DataAnnotations;
using LightApi.Infra.Extension;

namespace LightApi.Core.Dto;


public class PageQueryBaseDto:IValidatableObject
{
    /// <summary>
    /// 当前页 默认1
    /// </summary>
    [Range(1, int.MaxValue)]
    public int PageIndex { get; set; } = 1;
    
    /// <summary>
    /// 每页显示条数 默认20 最多400条
    /// </summary>
    [Range(1,400)]
    public int PageSize { get; set; } = 20;
    
    /// <summary>
    /// 排序字段 不分大小写
    /// </summary>
    public string? SortField { get; set; }
    /// <summary>
    /// 是否降序 
    /// </summary>
    public bool? IsDescending { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PageIndex < 1)
        {
            yield return new ValidationResult("PageIndex必须大于0");
        }
        if (PageSize < 1)
        {
            yield return new ValidationResult("PageSize必须大于0");
        }
        if(IsDescending is not null && string.IsNullOrWhiteSpace(SortField))
        {
            yield return new ValidationResult("IsDescending,SortField必须同时存在或不存在");
        }
        if(IsDescending is null && SortField.IsNotNullOrWhiteSpace())
        {
            yield return new ValidationResult("IsDescending,SortField必须同时存在或不存在");
        }
    }
    
    public virtual bool NeedOrder()
    {
        return SortField.IsNotNullOrWhiteSpace()&&IsDescending is not null;
    }
}
public class PageQueryBaseDto<T>:PageQueryBaseDto
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public T? Filter { get; set; }
}