using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagement.Api.Authentication.Jwt;
using UserManagement.Domain.Exceptions;
using UserManagement.Interfaces.Interfaces;

namespace UserManagement.Api;

public static class DependencyInjection
{
    public static void AddJwtAuthentication(
        this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        // getting settings

        JwtSettings settings = new();

        IConfigurationSection section = configurationManager.GetSection(JwtSettings.Section);
        if (!section.Exists())
            throw new ConfigurationSectionNotFoundException(JwtSettings.Section);

        section.Bind(settings);

        // adding services

        services.AddSingleton<IAuthenticationManager>(
            new JwtAuthenticationManager(settings)
            );

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = settings.CanTokenExpire,
                ValidateIssuerSigningKey = true,
                ValidIssuer = settings.Issuer,
                ValidAudience = settings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(settings.Key)),

                NameClaimType = "name"
            };
        });
    }
}
