using System.Collections;
using System.ComponentModel.DataAnnotations;
using Masuit.Tools.Core.Validator;

namespace LightApi.Infra.ModelValidator;

/// <summary>
/// 设置最小值校验
/// </summary>
public class MinValueExAttribute : ValidationAttribute
{
    /// <summary>
    /// 是否包含最小值
    /// </summary>
    public bool IncludeMin { get; set; } = true;
    public MinValueExAttribute(int min)
    {
        Minimum = min;
        
    }

  
    public int Minimum { get; set; }

    public override bool IsValid(object? value)
    {
        if(IncludeMin)
            return value == null || Convert.ToDouble(value) >= Minimum;
        
        return value == null || Convert.ToDouble(value) > Minimum;
    }

    public override string FormatErrorMessage(string name)
    {
        string mid=IncludeMin?"或等于":"";
        ErrorMessage ??= name+ $"必须大于{mid}" + Minimum ;
        return ErrorMessage;
    }

}