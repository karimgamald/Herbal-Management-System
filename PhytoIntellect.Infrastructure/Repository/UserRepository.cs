
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces.RepositoryInterfaces;
using PhytoIntellect.Infrastructure.Presistence;

namespace PhytoIntellect.Infrastructure.Repository;
public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }
    public async Task<User> GetByUserNameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }
}