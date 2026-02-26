using PhytoIntellect.Application.DTOs.UserDTOs;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PhytoIntellect.Application.Mappings
{
    internal class UserProfile : Profile
    {
        public UserProfile()
        {
            // 1. تحويل من Entity لـ DTO (عشان تعرض الداتا)
            CreateMap<User, UserDTO>();

            // 2. تحويل من DTO لـ Entity (عشان تسجل يوزر جديد مثلاً)
            CreateMap<RegisterUserDTO, User>();
        }
    }
}
