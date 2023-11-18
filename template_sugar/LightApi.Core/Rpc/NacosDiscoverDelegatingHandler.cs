using LightApi.Domain.Consts;
using Microsoft.Extensions.Logging;
using Nacos.V2;

namespace LightApi.Core.Rpc
{
    public class NacosDiscoverDelegatingHandler : DelegatingHandler
    {
        // private readonly ConsulClient _consulClient;
        private readonly ILogger<NacosDiscoverDelegatingHandler> _logger;
        private readonly INacosNamingService _svc;

        public NacosDiscoverDelegatingHandler( ILogger<NacosDiscoverDelegatingHandler> logger,Nacos.V2.INacosNamingService svc)
        {
            _logger = logger;
            _svc = svc;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentUri = request.RequestUri;
            if (currentUri is null)
                throw new NullReferenceException(nameof(request.RequestUri));
            var baseUri = currentUri.Host;
            _logger.LogDebug("请求地址 :{RequestRequestUri}", request.RequestUri);
            var healthyInstance = await _svc.SelectOneHealthyInstance(currentUri.Host,ServiceConsts.ServiceGroupName);
            if (healthyInstance is null)
            {
                throw new NullReferenceException($"{currentUri.Host}服务没有健康的节点!");
            }
            else
            {
                baseUri = string.Format("{0}:{1}",healthyInstance.Ip,healthyInstance.Port);
                var realRequestUri = new Uri($"{currentUri.Scheme}://{baseUri}{currentUri.PathAndQuery}");
                request.RequestUri = realRequestUri;
                _logger.LogDebug("请求真实地址:{RequestRequestUri}", request.RequestUri);
            }
                
            try
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.LogDebug(ex, "Exception during SendAsync()");
                throw;
            }
            finally
            {
                request.RequestUri = currentUri;
            }
        }
    }
}