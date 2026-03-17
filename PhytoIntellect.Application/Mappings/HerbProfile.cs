using AutoMapper;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.DTOs.HerbDTOs;
using PhytoIntellect.Core.Entities;

public class HerbProfile : Profile
{
    public HerbProfile()
    {
        CreateMap<HerbRequest, Herb>()
            .ForMember(dest => dest.ImageURL, opt => opt.Ignore());

        CreateMap<Herb, HerbResponse>();
        // ✅ هنا ضيف الخريطة الجديدة
        CreateMap<Herb, HerbWithHerbalistDto>()
            .ForMember(dest => dest.HerbalistId, opt => opt.MapFrom(src => src.AddedByHerbalist.HerbalistId))
            .ForMember(dest => dest.HerbalistName, opt => opt.MapFrom(src => src.AddedByHerbalist.User.FullName))
            .ForMember(dest => dest.ImageURL, opt => opt.Ignore()); // لأن IFormFile مش هيتحول من DB
    }
}