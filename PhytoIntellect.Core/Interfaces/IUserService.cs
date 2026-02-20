using Herbal_System.Entities;

namespace Herbal_System.Interfaces
{
    public interface IUserService
    {
        Task<User?> ValidateUserAsync(string username, string password);
    }

}
