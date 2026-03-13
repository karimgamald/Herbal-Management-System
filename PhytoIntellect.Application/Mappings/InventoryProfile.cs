using AutoMapper;
using PhytoIntellect.Application.Contracts.Inventory;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings;

public class InventoryProfile : Profile
{
    public InventoryProfile()
    {
        // Get herbname from => HerbalistHerb.Herb.HerbName
        CreateMap<HerbalistHerb, InventoryResponse>()
            .ForMember(dest => dest.HerbName,
                       opt => opt.MapFrom(src => src.Herb.HerbName));
    }
}