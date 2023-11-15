using FB.Filter;
using LightApi.Infra.AOP.Attributes;
using LightApi.Infra.AOP.Filters;
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

        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(val => $"输入值{val}无效"); 
        options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(val => $"缺少必要输入值"); 
        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(val => $"$输入值{val}无效"); 
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(val => $"$缺少必要输入值"); 
        options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => $"$缺少必要输入值"); 
        options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => $"$缺少必要输入值"); 
        options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => $"$缺少必要输入值"); 
        
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = _options.Value.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes;
    }


    
}