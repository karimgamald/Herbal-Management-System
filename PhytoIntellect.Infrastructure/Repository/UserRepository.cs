
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;
using System.Diagnostics.Metrics;

namespace PhytoIntellect.Infrastructure.Repository;

public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
{
    // هنا تقدر تستخدم context أو _dbSet براحتك
    // مثلا:
    // public async Task<bool> EmailExistsAsync(string email) 
    //     => await _dbSet.AnyAsync(u => u.Email == email);
}