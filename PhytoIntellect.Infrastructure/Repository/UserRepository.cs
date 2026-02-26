
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;
using System.Diagnostics.Metrics;

namespace PhytoIntellect.Infrastructure.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    
}