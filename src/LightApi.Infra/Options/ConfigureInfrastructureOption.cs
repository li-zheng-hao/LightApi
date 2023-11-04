using FB.Filter;
using FB.Infrastructure;
using FB.Infrastructure.AOP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LightApi.Infra.Options;

public class ConfigureInfrastructureOption :
    IConfigureOptions<MvcOptions>
{
    private readonly IConfiguration _configuration;

    private readonly IOptions<InfrastructureOptions> _options;

    public ConfigureInfrastructureOption(IConfiguration configuration, IOptions<InfrastructureOptions> options)
    {
        _configuration = configuration;
        _options = options;
    }

    public void Configure(MvcOptions options)
    {
        if (_options.Value.EnableGlobalExceptionFilter)
        {
            options.Filters.Add(typeof(GlobalExceptionsFilter));
        }
        if(_options.Value.EnableGlobalModelValidator)
        {
            options.Filters.Add(typeof(ModelValidatorFilter));
        }

        if (_options.Value.EnableGlobalUnifyResult)
        {
            options.Filters.Add(typeof(UnifyResultAttribute));
        }
        if (_options.Value.EnableGlobalAuthorize)
        {
            options.Filters.Add(typeof(AuthorizeAttribute));
        }
        
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = _options.Value.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes;
    }


    
}