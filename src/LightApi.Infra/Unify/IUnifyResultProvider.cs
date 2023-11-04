using System.Net;

namespace LightApi.Infra.Unify;

public interface IUnifyResultProvider
{
    public object Success(object? data, int code = 200, string? msg = "success",
        HttpStatusCode httpStatusCode = HttpStatusCode.OK);

    public object Failure(object? data, int code, string? msg,
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest);
}