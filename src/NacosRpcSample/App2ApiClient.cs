using Refit;

namespace NacosRpcSample;

public interface App2ApiClient
{
    [Get("/")]
    Task<string> GetHelloWorld();

    [Get("/api/ping")]
    Task<string> GetHelloWorld2();
}
