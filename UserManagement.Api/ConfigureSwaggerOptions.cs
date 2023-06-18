using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UserManagement.Api.Authentication.Jwt;

namespace UserManagement.Api;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
            options.SwaggerDoc(description.GroupName, CreateApiInfo(description));

        options.SetupForJwtBearer();
    }

    private OpenApiInfo CreateApiInfo(ApiVersionDescription versionDescription)
    {
        string apiVersion = versionDescription.ApiVersion.ToString();

        OpenApiInfo result = new OpenApiInfo
        {
            Version = apiVersion,
            Title = $"User Management MS {apiVersion}"
        };

        if (versionDescription.IsDeprecated)
            result.Description +=
                " This API version has been deprecated. Please use one of the new APIs available from the explorer.";

        return result;
    }
}

internal static class ConfigureSwaggerOptionsExtension
{
    public static void AddSwaggerOptions(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    }
}
