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

    public string msg { get; set; }

    public object? data { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    public HttpStatusCode httpStatusCode { get; set; } = HttpStatusCode.OK;

    public static UnifyResult Success()
    {
        return Success(null);
    }

    public static UnifyResult Success(object? data, int code = 200)
    {
        return new UnifyResult
        {
            code = code,
            data = data,
        };
    }


    public static UnifyResult Failure(string msg, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest,
        int code = 888)
    {
        return new UnifyResult
        {
            success = false,
            code = code,
            msg = msg,
            httpStatusCode = httpStatusCode,
        };
    }
}