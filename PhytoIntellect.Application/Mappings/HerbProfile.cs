using AutoMapper;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Core.Entities;

public class HerbProfile : Profile
{
    public HerbProfile()
    {
        CreateMap<HerbRequest, Herb>()
            .ForMember(dest => dest.ImageURL, opt => opt.Ignore());

        CreateMap<Herb, HerbResponse>();
    }
}