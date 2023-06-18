using UserManagement.Domain.Entities;

namespace UserManagement.Api.DataTransfer;

public record UserSimpleDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;
}

public static class UserToSimpleDtoExtension
{
    public static UserSimpleDto ToSimpleDto(this User user)
    {
        return new UserSimpleDto
        {
            Id = user.Id,
            Name = user.Name
        };
    }
}

