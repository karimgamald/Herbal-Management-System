using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.UserDTOs;

// للتعديل (من غير الباسورد، الباسورد ليه Endpoint لوحده للـ Reset)
public class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    //public string Role { get; set; } = string.Empty;
    public string? Governorate { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
}
