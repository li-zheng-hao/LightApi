using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using LightApi.Infra.InfraException;
using Masuit.Tools.Systems;

namespace LightApi.Core;

public enum BusinessErrorCode
{
    #region 通用错误

    [Description("请求错误")] Code400 = 400,
    [Description("暂无权限")] Code401 = 401,
    [Description("登录过期")] Code402 = 402,
    [Description("权限不足")] Code403 = 403,

    [Description("操作过于频繁,请稍后再试")] Code1000 = 1000,

    [Description("暂不支持本功能")] Code1001 = 1001,

    [Description("操作失败")] Code1002 = 1002,
    
    [Description("refresh_token无效")] Code1003 = 1003,
    
    [Description("业务繁忙,请稍后再试")] Code1004 = 1004,

    #endregion


    #region 数据错误

    [Description("数据格式存在错误")] Code100001 = 100001,

    [Description("数据不存在")] Code100002 = 100002,

    [Description("数据已存在")] Code100003 = 100003,

    [Description("{0}必须为数字")] Code100004 = 100004,

    [Description("{0}必须为整数")] Code100005 = 100005,

    [Description("{0}不能为空")] Code100006 = 100006,

    [Description("编号为{0}的数据不存在")] Code100007 = 100007,

    [Description("{0}不能重复")] Code100008 = 100008,

    [Description("{0}必须为正整数")] Code100009 = 100009,

    [Description("{0}不存在")] Code100010 = 100010,

    [Description("{0}已存在")] Code100011 = 100011,
    
    [Description("{0}错误")] Code100012 = 100012,
    
    [Description("{0}格式存在错误")] Code100013 = 100013,


    #endregion

    #region 用户相关

    [Description("用户名或密码不存在")] Code102000 = 102000,
    [Description("用户名已存在")] Code102001 = 102001,


    #endregion







}

public static class BusinessErrorCodeExtension
{
    /// <summary>
    /// 转换为异常 HTTP状态码 200
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static BusinessException ToBusinessException(this BusinessErrorCode code)
    {
        return new BusinessException(code.GetDescription(), (int)code);
    }

    /// <summary>
    /// 转换为异常 HTTP状态码 200
    /// </summary>
    /// <param name="code"></param>
    /// <param name="parameters">字符串参数 如{0}大于{1} 需要传两个参数代表0和1位置的数据</param>
    /// <returns></returns>
    public static BusinessException ToBusinessException(this BusinessErrorCode code, params object[] parameters)
    {
        var args = string.Format(code.GetDescription(), parameters);
        return new BusinessException(args, (int)code);
    }

    /// <summary>
    /// 转换为异常
    /// </summary>
    /// <param name="code"></param>
    /// <param name="httpStatusCode">http状态码</param>
    /// <param name="parameters">字符串参数 如{0}大于{1} 需要传两个参数代表0和1位置的数据</param>
    /// <returns></returns>
    public static BusinessException ToBusinessException(this BusinessErrorCode code, HttpStatusCode httpStatusCode,
        params object[] parameters)
    {
        var args = string.Format(code.GetDescription(), parameters);
        return new BusinessException(args, (int)code);
    }

    /// <summary>
    /// 手动校验dto
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="BusinessException"></exception>
    public static void ThrowIfValidateFailed(this object dto)
    {
        var context = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();

        bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(dto,context, validationResults, true);
                
        if(!isValid)
        {
            throw new BusinessException(validationResults[0].ErrorMessage?? "数据错误");
        }
    }
}