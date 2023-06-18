namespace UserManagement.Api.Authentication.Jwt;

internal class JwtSettings
{
    public const string Section = "Jwt";

    public int TokenValidityTime { get; init; }

    public string Issuer { get; init; } = null!;

    public string Audience { get; init; } = null!;

    public string Key { get; init; } = null!;

    public bool CanTokenExpire
    {
        get => TokenValidityTime > 0;
    }
}
