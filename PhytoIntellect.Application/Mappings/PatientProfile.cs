using AutoMapper;
using PhytoIntellect.Application.Contracts.MedicalHistories;
using PhytoIntellect.Application.Contracts.Patients;
using PhytoIntellect.Core.Entities;
using System;

namespace PhytoIntellect.Application.Mappings;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<Patient, PatientRequest>()
            .ForMember(dest => dest.GenderName, opt => opt.MapFrom(src => src.Gender.ToString()))
            // 🚀 استدعاء الدالة الـ static الجديدة بأمان هنا
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.BirthDate)))
            .ForMember(dest => dest.MedicalHistoryId, opt => opt.MapFrom(src => src.MedicalHistory != null ? src.MedicalHistory.MedicalHistoryId : (int?)null));

        CreateMap<PatientRequest, Patient>()
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => DateOnly.Parse(src.BirthDate)));

        CreateMap<MedicalHistory, MedicalHistoryResponse>();
    }

    // 🔒 تحويل الدالة إلى static حل مشكلة الـ Memory Leak وتعرّف الـ EF Core عليها تماماً
    public static int CalculateAge(DateOnly? birthDate)
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