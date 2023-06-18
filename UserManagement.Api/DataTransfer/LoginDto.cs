namespace UserManagement.Api.DataTransfer;

public record LoginDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public string Password { get; init; } = null!;
}
