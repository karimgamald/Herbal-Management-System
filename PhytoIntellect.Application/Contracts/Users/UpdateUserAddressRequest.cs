using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Users;

public class UpdateUserAddressRequest
{
    public string? Governorate { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
}
