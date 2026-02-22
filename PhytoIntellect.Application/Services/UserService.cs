using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;

namespace PhytoIntellect.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<string> RegisterUserAsync(RegisterUserDto request)
    {
        // 1. نتأكد إن الإيميل مش موجود قبل كدا
        var emailExists = await _userRepository.EmailExistsAsync(request.Email);
        if (emailExists)
        {
            return "Email is already registered. Please try logging in.";
        }

        // 2. نحول الـ DTO لـ Entity (لو معاك AutoMapper هيوفر السطور دي)
        var newUser = new User
        {
            FullName = request.FullName,
            UserName = request.UserName,
            Email = request.Email,
            // يفضل تشفر الباسورد هنا بـ BCrypt مثلاً
            // Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Password = request.Password, // مؤقتاً لحد ما تظبط التشفير
            Role = request.Role,
            Phone = request.Phone,
            CreatedAt = DateTime.Now
        };

        // 3. نحفظ في الداتابيز
        await _userRepository.AddAsync(newUser);

        return "User registered successfully.";
    }
}

