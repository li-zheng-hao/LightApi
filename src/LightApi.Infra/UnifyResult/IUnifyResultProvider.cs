using System.Net;

namespace LightApi.Infra.Unify;

public interface IUnifyResultProvider
{
    public IUnifyResult Success(
        object? data,
        int code = 200,
        string? msg = "success",
        HttpStatusCode httpStatusCode = HttpStatusCode.OK
    );

    public IUnifyResult Failure(
        object? data,
        int code,
        string? msg,
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest
    );

    public IUnifyResult Failure(
        object? data,
        int code,
        string? msg,
        object? extraInfo,
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest
    );
}
