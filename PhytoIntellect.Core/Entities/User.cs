using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
