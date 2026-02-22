using PhytoIntellect.Application.DTOs.UserDTOs;

namespace PhytoIntellect.Application.Interfaces;

public interface IUserService
{
    Task<string> RegisterUserAsync(RegisterUserDto request);
}
