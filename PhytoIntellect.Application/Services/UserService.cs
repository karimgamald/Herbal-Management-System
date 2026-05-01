using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.Accounts;
using PhytoIntellect.Application.Contracts.Users;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper) : IUserService
{
 
    public async Task<PaginatedList<UserResponse>> GetAllUsersAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.UserRepository.GetQueryable(tracked: false);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(u => u.FullName.ToLower().Contains(search) ||
                                     u.UserName.ToLower().Contains(search) ||
                                     u.Email.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
            query = filters.SortColumn.ToLower() 
            switch
            {
                "fullname" => isDesc ? query.OrderByDescending(u => u.FullName) : query.OrderBy(u => u.FullName),
                "email" => isDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                _ => query.OrderBy(u => u.Id)
            };
        }
        else
        {
            query = query.OrderBy(u => u.Id);
        }

        var projectedQuery = query.ProjectTo<UserResponse>(mapper.ConfigurationProvider);

        var paginatedUsers = await PaginatedList<UserResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return paginatedUsers;
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository.GetAsync(u => u.Id == id, tracked: false, cancellationToken: cancellationToken);
        return user == null ? null : mapper.Map<UserResponse>(user);
    }

    public async Task<string> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
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

    public async Task<string> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
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

    public async Task<RegisterUserAuthResponse> UpdateAddressAsync(int userId, UpdateUserAddressRequest model, CancellationToken cancellationToken = default)
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