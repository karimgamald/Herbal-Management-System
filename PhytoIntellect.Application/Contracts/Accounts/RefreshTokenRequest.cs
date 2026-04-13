using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Accounts
{
    public record RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
