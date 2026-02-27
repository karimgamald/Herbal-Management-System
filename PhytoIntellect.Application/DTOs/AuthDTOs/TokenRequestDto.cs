using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.AuthDTOs;

public class TokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
