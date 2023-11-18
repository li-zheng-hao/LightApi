using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LightApi.Core.Validator;

public class FileExtensionExAttribute : ValidationAttribute
{
    private readonly string[] _exts;

    /// <summary>
    /// 是否忽略大小写 默认为true
    /// </summary>
    public bool IgnoreCase { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exts">后缀名，带.不带.都可以</param>
    public FileExtensionExAttribute(params string[] exts)
    {
        _exts = exts;
    }

    /// <summary>
    /// 错误信息格式
    /// </summary>
    public string ErrorMessageFormat { get; set; } = "{0}文件后缀名必须为{1}";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var file = value as IFormFile;

        if (file is null)
        {
            return ValidationResult.Success;
        }

        var fileName = file.FileName;

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return new ValidationResult(ErrorMessage ?? "文件名不能为空");
        }

        var ext = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(ext))
        {
            return new ValidationResult(ErrorMessage ??
                                        string.Format(ErrorMessageFormat, fileName, string.Join(",", _exts)));
        }

        var success = _exts.Contains(ext, IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

        if (!success)
            return new ValidationResult(ErrorMessage ??
                                        string.Format(ErrorMessageFormat, fileName, string.Join(",", _exts)));

        return ValidationResult.Success;
    }
}