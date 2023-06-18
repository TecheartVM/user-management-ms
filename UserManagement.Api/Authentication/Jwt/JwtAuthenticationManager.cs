using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Domain.Entities;
using UserManagement.Interfaces.Interfaces;

namespace UserManagement.Api.Authentication.Jwt;

internal class JwtAuthenticationManager : IAuthenticationManager
{
    private readonly JwtSettings _settings;

    public JwtAuthenticationManager(JwtSettings settings)
    {
        _settings = settings;
    }

    public string? GenerateApiKey(User userData)
    {
        if(userData == null)
            return null;

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new()
        {
            new Claim("id", userData.Id.ToString()),
            new Claim("name", userData.Name)
        };

        var token = new JwtSecurityToken(
                _settings.Issuer,
                _settings.Audience,
                claims,
                expires: _settings.CanTokenExpire
                    ? DateTime.UtcNow.AddMinutes(_settings.TokenValidityTime)
                    : null,
                signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
