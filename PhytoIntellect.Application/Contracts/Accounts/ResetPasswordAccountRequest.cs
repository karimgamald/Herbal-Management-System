using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Accounts
{
    public class ResetPasswordAccountRequest
    {
        public string Email { get; set; } = string.Empty;
        public string? OldPassword { get; set; } // اختياري حسب السيستم
        public string NewPassword { get; set; } = string.Empty;
    }
}
