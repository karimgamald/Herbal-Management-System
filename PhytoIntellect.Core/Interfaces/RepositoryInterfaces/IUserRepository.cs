using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Core.Interfaces.RepositoryInterfaces;

public interface IUserRepository
{
    Task<User> GetByUserNameAsync(string username);
    Task<User> AddAsync(User user);
    Task<bool> EmailExistsAsync(string email);
}