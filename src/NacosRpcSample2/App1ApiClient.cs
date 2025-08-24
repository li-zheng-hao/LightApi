using Refit;

namespace NacosRpcSample;

public interface App1ApiClient
{
    [Get("/")]
    Task<string> GetHelloWorld();

    [Get("/api/ping")]
    Task<string> GetHelloWorld2();
}
