using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.AuthDTOs;

public class RegisterUserAuthDto
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // لازم يكون Patient أو Herbalist
    public string Phone { get; set; } = string.Empty;
}