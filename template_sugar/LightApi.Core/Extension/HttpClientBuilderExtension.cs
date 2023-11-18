using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace LightApi.Core.Extension;

public static class HttpClientBuilderExtension
{
    public static IHttpClientBuilder AddPolicyHandlerICollection(this IHttpClientBuilder builder, List<IAsyncPolicy<HttpResponseMessage>> policies)
    {
        policies?.ForEach(policy => builder.AddPolicyHandler(policy));
        return builder;
    }
}
