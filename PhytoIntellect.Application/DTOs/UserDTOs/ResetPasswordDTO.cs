using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.UserDTOs
{
    public class ResetPasswordDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
