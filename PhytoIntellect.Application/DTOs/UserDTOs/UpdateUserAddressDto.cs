using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.UserDTOs;

public class UpdateUserAddressDto
{
    public string? Governorate { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
}
