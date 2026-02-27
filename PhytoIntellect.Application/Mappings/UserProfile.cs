using AutoMapper;
using PhytoIntellect.Application.DTOs.AuthDTOs;
using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings;

internal class UserProfile : Profile
{
    public UserProfile()
    {
        // بتاعة الـ Auth
        CreateMap<User, UserAuthDto>();
        CreateMap<RegisterUserAuthDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());// بنشفره مانيوال

        // بتوع الـ Users Management
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}
