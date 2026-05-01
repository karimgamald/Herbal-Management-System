using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Core.Interfaces;

public interface IUserRepository
{
    IQueryable<User> GetQueryable(bool tracked = false);
}