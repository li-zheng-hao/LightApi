using System.Net;
using System.Text.Json.Serialization;

namespace LightApi.Infra.Unify;

/// <summary>
/// 通用返回包装类
/// </summary>
public class UnifyResult : IUnifyResult
{
    public bool success { get; set; } = true;

    public int code { get; set; }

    public string? msg { get; set; }

    public object? data { get; set; }

    /// <summary>
    /// 附加信息
    /// </summary>
    public object? extraInfo { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    public HttpStatusCode httpStatusCode { get; set; } = HttpStatusCode.OK;

    public static UnifyResult Success()
    {
        return Success(null);
    }

    public static UnifyResult Success(object? data, int code = 200)
    {
        return new UnifyResult { code = code, data = data, };
    }

    public static UnifyResult Failure(
        string? msg,
        object? data,
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest,
        int code = 888
    )
    {
        return new UnifyResult
        {
            success = false,
            code = code,
            msg = msg,
            data = data,
            httpStatusCode = httpStatusCode,
        };
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="data"></param>
    /// <param name="extraInfo"></param>
    /// <param name="httpStatusCode"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public static UnifyResult Failure(
        string? msg,
        object? data,
        object? extraInfo,
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest,
        int code = 888
    )
    {
        return new UnifyResult
        {
            success = false,
            code = code,
            msg = msg,
            data = data,
            extraInfo = extraInfo,
            httpStatusCode = httpStatusCode,
        };
    }
}
