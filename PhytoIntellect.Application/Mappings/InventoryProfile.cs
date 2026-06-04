using AutoMapper;
using PhytoIntellect.Application.Contracts.Inventory;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings;

public class InventoryProfile : Profile
{
    public InventoryProfile()
    {
        CreateMap<HerbalistHerb, InventoryResponse>()
            .ForMember(dest => dest.HerbName, opt => opt.MapFrom(src => src.Herb.HerbName));
        CreateMap<HerbalistHerb, InventoryResponse>()
            // 🚀 ربط الحقل مباشرة من الـ Entity إلى الـ Response DTO
            .ForMember(dest => dest.HerbalistId, opt => opt.MapFrom(src => src.HerbalistId))
            .ForMember(dest => dest.HerbName, opt => opt.MapFrom(src => src.Herb.HerbName));
    }
}