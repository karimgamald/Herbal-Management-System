using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Core.Interfaces;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<bool> EmailExistsAsync(string email);
}