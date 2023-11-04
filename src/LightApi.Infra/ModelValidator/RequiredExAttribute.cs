using System.ComponentModel.DataAnnotations;
using LightApi.Infra.Extension;
using Masuit.Tools;

namespace LightApi.Infra.ModelValidator;

/// <summary>
/// 增强版Required 默认提示为："{DisplayName}不能为空"  支持字符串判空和空白  数组判空 对象判断null
/// </summary>
public class RequiredExAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        bool passed = true;


        if (value is string stringValue)
        {
            passed = !stringValue.IsNullOrWhiteSpace();
        }
        else if (value.IsNullOrEmpty())
        {
            passed = false;
        }

        if (!passed)
        {
            if (ErrorMessage.IsNullOrWhiteSpace())
                return new ValidationResult($"{validationContext.DisplayName}不能为空");
            return new ValidationResult(this.ErrorMessage);
        }

        return ValidationResult.Success;
    }
}