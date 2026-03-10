using AutoMapper;
using PhytoIntellect.Application.DTOs.PatientDTOs;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.GenderName, opt => opt.MapFrom(src => src.Gender.ToString()))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.BirthDate)));

        CreateMap<PatientDto, Patient>()
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => DateOnly.Parse(src.BirthDate)));
    }

    private int CalculateAge(DateOnly? birthDate)
    {
        if (!birthDate.HasValue) return 0;

        var today = DateOnly.FromDateTime(DateTime.Now);
        var age = today.Year - birthDate.Value.Year;

        if (birthDate.Value > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}