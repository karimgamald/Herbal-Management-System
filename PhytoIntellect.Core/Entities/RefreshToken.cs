using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
