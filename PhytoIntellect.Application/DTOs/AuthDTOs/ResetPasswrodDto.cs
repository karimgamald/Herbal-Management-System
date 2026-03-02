using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.AuthDTOs
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string? OldPassword { get; set; } // اختياري حسب السيستم
        public string NewPassword { get; set; } = string.Empty;
    }
}
