using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ReverseProxy.Swagger;

namespace LightApi.Gateway.Swagger;

public class ConfigureSwaggerOptions(
    IOptionsMonitor<ReverseProxyDocumentFilterConfig> reverseProxyDocumentFilterConfigOptions)
    : IConfigureOptions<SwaggerGenOptions>
{
    private readonly ReverseProxyDocumentFilterConfig _reverseProxyDocumentFilterConfig = reverseProxyDocumentFilterConfigOptions.CurrentValue;

    public void Configure(SwaggerGenOptions options)
    {
        var filterDescriptors = new List<FilterDescriptor>();

        options.ConfigureSwaggerDocs(_reverseProxyDocumentFilterConfig);

        filterDescriptors.Add(new FilterDescriptor
        {
            Type = typeof(NacosReverseProxyDocumentFilter),
            Arguments = []
        });

        options.DocumentFilterDescriptors = filterDescriptors;
    }
}