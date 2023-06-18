using UserManagement.Domain.Entities;

namespace UserManagement.Interfaces.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> Get();

    Task<User?> Get(Guid id);

    Task<User?> Add(User user);

    Task<bool> Update(User user);

    Task<bool> Delete(Guid id);
}

