using System.ComponentModel.DataAnnotations;
using UserManagement.Api.Validation.Attributes;
using UserManagement.Domain.Entities;

namespace UserManagement.Api.DataTransfer;

public class UserPutDto
{
    [GuidId]
    public Guid Id { get; init; }

    [StringLength(200)]
    public string Name { get; init; } = null!;

    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public string Email { get; init; } = null!;

    [DataType(DataType.Password)]
    [StringLength(maximumLength: 100, MinimumLength = 8)]
    public string Password { get; init; } = null!;

    [DataType(DataType.PhoneNumber)]
    [Phone]
    public string Phone { get; init; } = null!;

    public virtual User CreateUser()
    {
        return new User
        {
            Id = this.Id,
            Name = this.Name,
            Email = this.Email,
            Password = this.Password,
            Phone = this.Phone,
            Created = DateTime.UtcNow
        };
    }
}
