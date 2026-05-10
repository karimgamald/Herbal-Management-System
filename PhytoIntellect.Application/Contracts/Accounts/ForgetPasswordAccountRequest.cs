using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Accounts
{
    public record ForgetPasswordAccountRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
