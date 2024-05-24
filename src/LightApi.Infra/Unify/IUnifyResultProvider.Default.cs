using System.Net;
using LightApi.Infra.Options;

namespace LightApi.Infra.Unify;

/// <summary>
/// 
/// </summary>
public class UnifyResultProvider : IUnifyResultProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="code"></param>
    /// <param name="msg"></param>
    /// <param name="httpStatusCode"></param>
    /// <returns></returns>
    public IUnifyResult Success(object? data, int code = 200, string? msg = "success",
        HttpStatusCode httpStatusCode = HttpStatusCode.OK)
    {
        return UnifyResult.Success(data, code);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="code"></param>
    /// <param name="msg"></param>
    /// <param name="httpStatusCode"></param>
    /// <returns></returns>
    public IUnifyResult Failure(object? data, int code, string? msg,
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
    {
        return UnifyResult.Failure(msg,data, httpStatusCode, code);
    }

    public IUnifyResult Failure(object? data, int code, string? msg, object? extraInfo,
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
    {
        return UnifyResult.Failure(msg,data,extraInfo, httpStatusCode, code);
    }
}