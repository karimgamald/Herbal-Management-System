using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.UserDTOs;

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; } 
}