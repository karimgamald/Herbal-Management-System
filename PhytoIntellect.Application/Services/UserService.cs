using AutoMapper;
using PhytoIntellect.Application.DTOs.AuthDTOs;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await unitOfWork.UserRepository.GetAllAsync(tracked: false, cancellationToken: cancellationToken);
        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == id, tracked: false, cancellationToken);
        return user == null ? null : mapper.Map<UserDto>(user);
    }

    public async Task<string> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        // 1. Validation للـ Role
        if (!AppRoles.IsValidRole(request.Role))
            return $"Invalid Role. Must be '{AppRoles.Patient}' or '{AppRoles.Herbalist}'.";

        // 2. Validation لليوزرنيم
        var existingUser = await unitOfWork.UserRepository.GetAsync(u => u.Email == request.Email, 
            tracked: false, cancellationToken);
        if (existingUser != null) return "Username is already taken.";

        var user = mapper.Map<User>(request);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await unitOfWork.UserRepository.CreateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "User created successfully.";
    }

    public async Task<string> UpdateUserAsync(int id, UpdateUserDto request, 
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == id, tracked: true, cancellationToken);
        if (user == null) return "User not found.";

        if (!AppRoles.IsValidRole(request.Role))
            return $"Invalid Role. Must be '{AppRoles.Patient}' or '{AppRoles.Herbalist}'.";

        // تحديث الداتا
        user.FullName = request.FullName;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Role = request.Role;

        unitOfWork.UserRepository.Update(user); // مفيش await عشان دي Void
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "User updated successfully.";
    }

    public async Task<string> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == id, tracked: true, cancellationToken);
        if (user == null) return "User not found.";

        unitOfWork.UserRepository.Remove(user); // مفيش await
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "User deleted successfully.";
    }

    public async Task<AuthResultDto> UpdateAddressAsync(int userId, UpdateUserAddressDto model, CancellationToken cancellationToken = default)
    {
        // بنجيب اليوزر من Repository اليوزر نفسه
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == userId, tracked: true, cancellationToken);

        if (user == null)
            return new AuthResultDto { Success = false, Message = "User not found." };

        // تحديث البيانات (حتى لو مبعوتة بـ null هتتحدث عادي)
        user.Governorate = model.Governorate;
        user.City = model.City;
        user.Street = model.Street;

        unitOfWork.UserRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto { Success = true, Message = "User address updated successfully." };
    }
}