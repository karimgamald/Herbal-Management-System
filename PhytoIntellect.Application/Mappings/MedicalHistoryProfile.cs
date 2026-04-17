using AutoMapper;
using PhytoIntellect.Application.Contracts.MedicalHistories;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class MedicalHistoryProfile : Profile
{
    public MedicalHistoryProfile()
    {
        CreateMap<MedicalHistory, MedicalHistoryResponse>();

        CreateMap<MedicalHistoryRequest, MedicalHistory>();
    }
}
