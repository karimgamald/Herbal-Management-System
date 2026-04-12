using AutoMapper;
using PhytoIntellect.Application.Contracts.Accounts;
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
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == id, tracked: false, cancellationToken: cancellationToken);
        return user == null ? null : mapper.Map<UserDto>(user);
    }

    public async Task<string> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        // 1. Validation للـ Role
        if (!AppRoles.IsValidRole(request.Role))
            return $"Invalid Role. Must be '{AppRoles.Patient}' or '{AppRoles.Herbalist}'.";

        // 2. Validation لليوزرنيم
        var existingUser = await unitOfWork.UserRepository.GetAsync(u => u.Email == request.Email, 
            tracked: false, cancellationToken: cancellationToken);
        if (existingUser != null) 
            return "Username is already taken.";

        var user = mapper.Map<User>(request);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await unitOfWork.UserRepository.CreateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "User created successfully.";
    }

    public async Task<string> UpdateUserAsync(int id, UpdateUserDto request, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == id,tracked: true, cancellationToken: cancellationToken);

        if (user == null)
            return "User not found.";

        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;

        if (!string.IsNullOrWhiteSpace(request.UserName))
            user.UserName = request.UserName;

        if (!string.IsNullOrWhiteSpace(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.Phone))
            user.Phone = request.Phone;

        if (!string.IsNullOrWhiteSpace(request.Governorate))
            user.Governorate = request.Governorate;

        if (!string.IsNullOrWhiteSpace(request.City))
            user.City = request.City;

        if (!string.IsNullOrWhiteSpace(request.Street))
            user.Street = request.Street;

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "User updated successfully.";
    }
    public async Task<string> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == id, tracked: true, cancellationToken: cancellationToken);
        if (user == null) return "User not found.";

        unitOfWork.UserRepository.Remove(user); // مفيش await
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "User deleted successfully.";
    }

    public async Task<RegisterUserAuthResponse> UpdateAddressAsync(int userId, UpdateUserAddressDto model, CancellationToken cancellationToken = default)
    {
        // بنجيب اليوزر من Repository اليوزر نفسه
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == userId, tracked: true, cancellationToken: cancellationToken);

        if (user == null)
            return new RegisterUserAuthResponse { Success = false, Message = "User not found." };

        // تحديث البيانات (حتى لو مبعوتة بـ null هتتحدث عادي)
        if (!string.IsNullOrWhiteSpace(model.Governorate) && model.Governorate != "string")
            user.Governorate = model.Governorate;

        if (!string.IsNullOrWhiteSpace(model.City) && model.City != "string")
            user.City = model.City;

        if (!string.IsNullOrWhiteSpace(model.Street) && model.Street != "string")
            user.Street = model.Street;

        unitOfWork.UserRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterUserAuthResponse { Success = true, Message = "User address updated successfully." };
    }
}