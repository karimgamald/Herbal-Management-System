using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        Task<User?> ValidateByUserNameAsync(string username);
    }
}
