using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Accounts
{
    public class ResetPasswordAccountRequest
    {
        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string? OldPassword { get; set; }   // optional

        public string? Token { get; set; }         // optional (forgot password flow)
    }
}
