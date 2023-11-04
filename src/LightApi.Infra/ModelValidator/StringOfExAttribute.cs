using System.ComponentModel.DataAnnotations;

namespace LightApi.Infra.ModelValidator;

/// <summary>
/// 字符串包含校验
/// </summary>
public class StringOfExAttribute : ValidationAttribute
{
    public string[] AllowableValues { get; set; }
    
    /// <summary>
    /// 是否允许null或空白字符串
    /// </summary>
    public bool AllowNullOrWhiteSpace { get; set; } = true;

    public StringOfExAttribute(params string[] allowValues)
    {
        AllowableValues = allowValues;
    }

    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        var msg = $"请输入允许的值: {string.Join(", ", (AllowableValues ?? new string[] { "没有发现允许的值" }))}.";

        if(string.IsNullOrWhiteSpace(value.ToString()))
        {
            return AllowNullOrWhiteSpace ? ValidationResult.Success : new ValidationResult(msg);
        }
       
        if (AllowableValues?.Contains(value?.ToString()) == true)
        {
            return ValidationResult.Success;
        }


        return new ValidationResult(msg);
    }
    
}