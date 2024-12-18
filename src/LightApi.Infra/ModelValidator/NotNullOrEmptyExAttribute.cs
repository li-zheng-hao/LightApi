using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace LightApi.Infra.ModelValidator;

/// <summary>
/// 数组不能为空 字符串不能是空或者空白
/// </summary>
public class NotNullOrEmptyExAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage ??= $"{validationContext.DisplayName}不能为空";

        if (value == null)
        {
            return new ValidationResult(ErrorMessage);
        }

        // 判断是字符串还是数组，如果是字符串则判断长度，如果是数组则判断数组长度
        if (value is string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return new ValidationResult(ErrorMessage);
            }
        }
        else if (value is ICollection { Count: 0 })
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
