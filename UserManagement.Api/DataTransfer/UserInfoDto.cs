using System.Text.Json.Serialization;
using UserManagement.Domain.Entities;

namespace UserManagement.Api.DataTransfer;

public record UserInfoDto
{
    public Guid Id { get; init; }

    public string Email { get; init; } = null!;

    public string Name { get; init; } = null!;

    public string? Phone { get; init; }

    [JsonPropertyName("registration_date")]
    public DateTime RegistrationDate { get; init; }
}

public static class UserToInfoDtoExtension
{
    public static UserInfoDto ToInfoDto(this User user)
    {
        return new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Phone = user.Phone,
            RegistrationDate = user.Created
        };
    }
}

