using AutoMapper;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Application.DTOs.HerbalistDTOs;

namespace PhytoIntellect.Application.Mappings
{
    public class HerbalistProfile : Profile
    {
        public HerbalistProfile()
        {
            // تحويل من Entity إلى DTO
            CreateMap<Herbalist, HerbalistDto>();

            // لو عندك Create/Update DTOs
            CreateMap<CreateOrUpdateHerbalistDto, Herbalist>();
        }
    }
}