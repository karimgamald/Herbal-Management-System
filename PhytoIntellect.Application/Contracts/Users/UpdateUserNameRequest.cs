using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Users;

public class UpdateUserNameRequest
{
    public string UserName { get; set; } = string.Empty;
}