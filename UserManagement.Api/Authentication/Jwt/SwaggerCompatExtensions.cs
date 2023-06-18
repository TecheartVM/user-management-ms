using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace UserManagement.Api.Authentication.Jwt;

internal static class SwaggerCompatExtensions
{
    public static void SetupForJwtBearer(this SwaggerGenOptions options, string? apiVersion = null)
    {
        string securitySchemeName = "AuthToken";

        if (apiVersion != null)
            securitySchemeName += $" {apiVersion}";

        var securityScheme = new OpenApiSecurityScheme()
        {
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            Name = "Authorization",
            Description = "JWT Authorization using the Bearer scheme",

            Reference = new OpenApiReference
            {
                Id = securitySchemeName,
                Type = ReferenceType.SecurityScheme
            }
        };

        options.AddSecurityDefinition(securitySchemeName, securityScheme);

        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                { securityScheme, Array.Empty<string>() }
            });

        options.CustomOperationIds(apiDescription => 
            apiDescription.TryGetMethodInfo(out MethodInfo methodInfo)
                ? methodInfo.Name
                : null);
    }
}
