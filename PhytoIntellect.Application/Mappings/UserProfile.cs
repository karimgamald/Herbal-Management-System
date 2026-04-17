using AutoMapper;
using PhytoIntellect.Application.Contracts.Accounts;
using PhytoIntellect.Application.Contracts.Users;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings;

internal class UserProfile : Profile
{
    public UserProfile()
    {
        // بتاعة الـ Auth
        CreateMap<User, RegisterUserAuthRequest>();
        CreateMap<RegisterUserAuthRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());// بنشفره مانيوال

        // بتوع الـ Users Management
        CreateMap<User, UserResponse>();
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}
