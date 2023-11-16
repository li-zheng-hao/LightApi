using System.Net;
using LightApi.Infra.Options;

namespace LightApi.Infra.Unify;

/// <summary>
/// 
/// </summary>
public class DefaultUnifyResultProvider : IUnifyResultProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="code"></param>
    /// <param name="msg"></param>
    /// <param name="httpStatusCode"></param>
    /// <returns></returns>
    public object Success(object? data, int code = 200, string? msg = "success",
        HttpStatusCode httpStatusCode = HttpStatusCode.OK)
    {
        return Unify.UnifyResult.Success(data, code);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="code"></param>
    /// <param name="msg"></param>
    /// <param name="httpStatusCode"></param>
    /// <returns></returns>
    public object Failure(object? data, int code, string? msg,
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
    {
        return Unify.UnifyResult.Failure(msg,data, httpStatusCode, code);
    }
}