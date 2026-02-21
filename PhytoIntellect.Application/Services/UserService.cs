using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                return null;

            // Password verification (hashed)
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }
    }
}
