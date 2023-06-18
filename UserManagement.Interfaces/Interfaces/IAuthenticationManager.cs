using UserManagement.Domain.Entities;

namespace UserManagement.Interfaces.Interfaces;

public interface IAuthenticationManager
{
    string? GenerateApiKey(User userData);
}
