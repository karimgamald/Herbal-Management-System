using AutoMapper;
using PhytoIntellect.Application.DTOs.PatientDTOs;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<Patient, PatientDto>()
            // بنحول الـ Enum بتاع الـ Gender لـ String عشان يتقري بسهولة في الموبايل
            .ForMember(dest => dest.GenderName, opt => opt.MapFrom(src => src.Gender.ToString()));
    }
}