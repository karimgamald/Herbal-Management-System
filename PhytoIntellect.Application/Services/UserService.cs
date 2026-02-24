using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;

namespace PhytoIntellect.Application.Services;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        var user = await _userRepository.GetByUserNameAsync(username);

        if (user == null)
            return null;

        // Password verification (hashed)
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
    public async Task<User?> ValidateByUserNameAsync(string username)
    {
        var user = await _userRepository.GetByUserNameAsync(username);

        if (user == null)
            return null;

        return user;
    }

    public async Task<string> RegisterUserAsync(RegisterUserDTO request) // Here Using Auto Mapper
    {
        var emailExists = await _userRepository.EmailExistsAsync(request.Email);
        if (emailExists) return "Email is already registered.";

        // السطر السحري: حول الـ request (DTO) لـ User (Entity) في خطوة واحدة!
        var newUser = _mapper.Map<User>(request);

        // متنساش تحط الحاجات اللي مش بتيجي من الـ DTO
        newUser.CreatedAt = DateTime.Now;

        await _userRepository.AddAsync(newUser);
        return "User registered successfully.";
    }
}
