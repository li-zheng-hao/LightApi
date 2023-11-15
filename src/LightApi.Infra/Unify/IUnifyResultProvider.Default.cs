using System.Net;
using LightApi.Infra.Options;

namespace LightApi.Infra.Unify;

public class DefaultUnifyResultProvider : IUnifyResultProvider
{
    public object Success(object? data, int code = 200, string? msg = "success",
        HttpStatusCode httpStatusCode = HttpStatusCode.OK)
    {
        return Unify.UnifyResult.Success(data, code);
    }

    public object Failure(object? data, int code, string? msg,
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
    {
        return Unify.UnifyResult.Failure(msg, httpStatusCode, code);
    }
}