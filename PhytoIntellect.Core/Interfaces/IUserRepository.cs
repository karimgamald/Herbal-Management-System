using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Interfaces;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<bool> EmailExistsAsync(string email);
}