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
                .ForMember(dest => dest.HerbalistName, opt => opt.MapFrom(src => src.Herbalist!.User!.FullName))
                .ForMember(dest => dest.Herbs, opt => opt.MapFrom(src => src.OrderHerbs))
                .ForMember(dest => dest.Recipes, opt => opt.MapFrom(src => src.OrderRecipes))
                .ForMember(dest => dest.AiRecipes, opt => opt.MapFrom(src => src.OrderAiRecipes));
    }
}