using AutoMapper;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Application.DTOs.HerbalistDTOs;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings
{
    public class HerbalistProfile : Profile
    {
        public HerbalistProfile()
        {
            CreateMap<Herbalist, HerbalistDto>();
            CreateMap<CreateOrUpdateHerbalistDto, Herbalist>();
        }
    }
}