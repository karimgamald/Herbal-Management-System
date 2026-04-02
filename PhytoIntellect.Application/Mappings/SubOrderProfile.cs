using AutoMapper;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Core.Entities;

public class SubOrderProfile : Profile
{
    public SubOrderProfile()
    {
        CreateMap<SubOrder, SubOrderSummaryResponse>();

        CreateMap<SubOrder, SubOrderDetailsResponse>()
                .ForMember(dest => dest.TrackingNumber, opt => opt.MapFrom(src => src.ExternalDeliveryID))
                .ForMember(dest => dest.Recipes, opt => opt.MapFrom(src => src.OrderRecipes))
                .ForMember(dest => dest.Herbs, opt => opt.MapFrom(src => src.OrderHerbs));
    }
}