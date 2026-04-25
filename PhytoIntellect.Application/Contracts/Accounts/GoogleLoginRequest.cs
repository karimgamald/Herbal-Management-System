using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Accounts;

public class GoogleLoginRequest
{
    public string IdToken { get; set; } = string.Empty;
    public string? Role { get; set; }
}