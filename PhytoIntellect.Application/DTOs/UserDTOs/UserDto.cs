using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.UserDTOs;

// للعرض (زي ما هي مفيهاش باسورد)
public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
}