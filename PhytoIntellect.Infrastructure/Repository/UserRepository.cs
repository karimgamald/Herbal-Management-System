
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;
using System.Diagnostics.Metrics;

namespace PhytoIntellect.Infrastructure.Repository;

public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
{
    // public async Task<bool> EmailExistsAsync(string email) 
    //     => await _dbSet.AnyAsync(u => u.Email == email);

    public IQueryable<User> GetQueryable(bool tracked = false)
    {
        return tracked ? context.Users : context.Users.AsNoTracking();
    }
}