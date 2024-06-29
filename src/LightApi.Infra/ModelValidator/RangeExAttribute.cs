using System.ComponentModel.DataAnnotations;

namespace LightApi.Infra.ModelValidator;

/// <summary>
/// 增强版Required 默认提示为：{DisplayName}不能为空 支持double和int
/// </summary>
public class RangeExAttribute : RequiredAttribute
{
    public object Minimum;

    public object Maximum;

    private Type OperandType;

    /// <summary>
    /// 包含Minimum和Maximum
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="ErrorMessage"></param>
    public RangeExAttribute(int min, int max)
    {
        Minimum = min;
        Maximum = max;
        OperandType = typeof(int);
    }

    /// <summary>
    /// 包含Minimum和Maximum
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="ErrorMessage"></param>
    public RangeExAttribute(double min, double max)
    {
        Minimum = min;
        Maximum = max;
        OperandType = typeof(double);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage ??= validationContext.DisplayName + "必须在" + Minimum + "和" + Maximum + "之间";

        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (OperandType == typeof(int))
            return ValidateInt(value);

        if (OperandType == typeof(double))
            return ValidateDouble(value);

        var converted=double.TryParse(value.ToString(), out var result);
        
        if (converted)
        {
            if (result >= (double)Minimum && result <= (double)Maximum)
                return ValidationResult.Success;
        }
        
        return ValidationResult.Success;
    }

    private ValidationResult ValidateDecimal(object value)
    {
        bool passed = true;
        if (value is string str)
        {
            passed = decimal.TryParse(str, out var result);
            if (passed)
                passed = result >= (decimal)Minimum && result <= (decimal)Maximum;
        }
        else
        {
            try
            {
                var result = Convert.ToDecimal(value);

                passed = result >= (decimal)Minimum && result <= (decimal)Maximum;
            }
            catch (Exception)
            {
                passed = false;
            }
        }


        return passed ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }


    private ValidationResult ValidateDouble(object value)
    {
        bool passed = true;
        if (value is string str)
        {
            passed = double.TryParse(str, out var result);
            if (passed)
                passed = result >= (double)Minimum && result <= (double)Maximum;
        }
        else
        {
            try
            {
                var result = Convert.ToDouble(value);

                passed = result >= (double)Minimum && result <= (double)Maximum;
            }
            catch (Exception)
            {
                passed = false;
            }
        }


        return passed ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }

    ValidationResult ValidateInt(object value)
    {
        bool passed = true;
        if (value is string str)
        {
            passed = int.TryParse(str, out var result);
            if (passed)
                passed = result >= (int)Minimum && result <= (int)Maximum;
        }
        else
        {
            try
            {
                var result = Convert.ToInt32(value);

                passed = result >= (int)Minimum && result <= (int)Maximum;
            }
            catch (Exception)
            {
                passed = false;
            }
        }

        return passed ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }
}