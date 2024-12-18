using System.ComponentModel.DataAnnotations;

namespace LightApi.Infra.ModelValidator;

/// <summary>
/// 枚举值校验
/// </summary>
public class EnumOfExAttribute : ValidationAttribute
{
    private Type Type { get; set; }

    /// <summary>
    /// 枚举类型
    /// </summary>
    /// <param name="value"></param>
    public EnumOfExAttribute(Type value)
    {
        Type = value;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage ??= $"{validationContext.DisplayName}填写错误";

        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (!Enum.IsDefined(Type, value))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
