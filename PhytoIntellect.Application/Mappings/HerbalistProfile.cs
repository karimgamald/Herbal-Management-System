using AutoMapper;
using PhytoIntellect.Application.Contracts.Herbalists;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings
{
    public class HerbalistProfile : Profile
    {
        public HerbalistProfile()
        {
            CreateMap<Herbalist, HerbalistResponse>();
            CreateMap<CreateOrUpdateHerbalistRequest, Herbalist>();
        }
    }
}