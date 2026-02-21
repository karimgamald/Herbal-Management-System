using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PhytoIntellect.Core.Entities
{
    public class TokenHasher
    {
        public static string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

    }
}
