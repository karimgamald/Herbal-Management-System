using AutoMapper;
using PhytoIntellect.Application.DTOs.MedicalHistoryDTOs;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class MedicalHistoryProfile : Profile
{
    public MedicalHistoryProfile()
    {
        CreateMap<MedicalHistory, MedicalHistoryDto>();

        CreateMap<ManageMedicalHistoryDto, MedicalHistory>();
    }
}
