using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Interfaces
{
    public interface IUserService
    {
        Task<User?> ValidateUserAsync(string username, string password);
    }
}
