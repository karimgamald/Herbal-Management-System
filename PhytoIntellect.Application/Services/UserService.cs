using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Core.Interfaces.RepositoryInterfaces;

namespace PhytoIntellect.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        var user = await _userRepository.GetByUserNameAsync(username);

        if (user == null)
            return null;

        // Password verification (hashed)
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
    public async Task<User?> ValidateByUserNameAsync(string username)
    {
        var user = await _userRepository.GetByUserNameAsync(username);

        if (user == null)
            return null;

        return user;
    }
}

