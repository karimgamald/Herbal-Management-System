using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Interfaces;

public interface IHerbalistRepository : IRepository<Herbalist>
{
    Task<int> GetIdByUserIdAsync(string userId);
}