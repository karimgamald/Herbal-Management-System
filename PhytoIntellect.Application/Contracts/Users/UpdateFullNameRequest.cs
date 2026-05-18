using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Users;

public class UpdateFullNameRequest
{
    public string FullName { get; set; } = string.Empty;
}