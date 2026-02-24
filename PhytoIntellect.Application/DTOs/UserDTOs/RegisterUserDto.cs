using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.UserDTOs;

public class RegisterUserDTO
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}