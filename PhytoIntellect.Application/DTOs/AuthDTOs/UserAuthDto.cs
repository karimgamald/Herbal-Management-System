using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.AuthDTOs;

public class UserAuthDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
