using System.Collections;
using System.ComponentModel.DataAnnotations;
using Masuit.Tools;

namespace LightApi.Infra.ModelValidator;

/// <summary>
/// 计算字符串或者数组的长度
/// </summary>
public class LengthExAttribute : ValidationAttribute
{
    public LengthExAttribute(int min, int max)
    {
        Minimum = min;
        Maximum = max;
    }

    public LengthExAttribute(int min)
    {
        Minimum = min;
        Maximum = int.MaxValue;
    }

    public int Maximum { get; set; }

    public int Minimum { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage ??= $"{validationContext.DisplayName}长度必须在{Minimum}和{Maximum}之间";

        if (value.IsNullOrEmpty()) return ValidationResult.Success;
        
        // 判断是字符串还是数组，如果是字符串则判断长度，如果是数组则判断数组长度
        if (value is string stringValue)
        {
            if (stringValue.Length < Minimum || stringValue.Length > Maximum)
            {
                return new ValidationResult(ErrorMessage);
            }
        }
        else if (value is ICollection arr)
        {
            if (arr.Count < Minimum || arr.Count > Maximum)
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}