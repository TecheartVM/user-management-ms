namespace UserManagement.Api.DataTransfer;

public record UserListDto
{
    public int Count { get; init; }

    public IEnumerable<UserSimpleDto> Users { get; init; } = null!;
}