using AutoMapper;
using PhytoIntellect.Application.Contracts.Diseases;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings;

public class DiseaseProfile : Profile
{
    public DiseaseProfile()
    {
        // تحويل من الداتابيز للريسبونس
        CreateMap<Disease, DiseaseResponse>();
    }
}