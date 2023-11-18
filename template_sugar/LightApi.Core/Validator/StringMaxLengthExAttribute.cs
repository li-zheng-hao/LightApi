using System.ComponentModel.DataAnnotations;

namespace LightApi.Core.Validator;

/// <summary>
/// 最大长度验证 支持string
/// </summary>
public class StringMaxLengthExAttribute:ValidationAttribute
{
    private readonly uint _length;

    public StringMaxLengthExAttribute(uint length)
    {
        _length = length;
    }

    /// <summary>
    /// 错误信息格式
    /// </summary>
    public string ErrorMessageFormat { get; set; } = "{0}长度不能超过{1}个字符";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        bool success = true;

        if (value is string str)
        {
            success = str.Length <= _length;
        }

        if (!success)
        {
            return new ValidationResult(ErrorMessage ??
                                        string.Format(ErrorMessageFormat, validationContext.DisplayName, _length));
        }

     

        return ValidationResult.Success;
    }
}