using AutoMapper;
using PhytoIntellect.Application.Contracts.ChatAiRecipes;
using PhytoIntellect.Application.Contracts.UserFavorites;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings;

public class AiChatRecipeProfile : Profile
{
    public AiChatRecipeProfile()
    {
        CreateMap<AiChatRecipe, AiChatPredictionResult>()
            .ForMember(dest => dest.AiChatRecipeId, opt => opt.MapFrom(src => src.Id));

        CreateMap<AiChatRecipe, FavoriteResponse>()
        .ForMember(dest => dest.TargetId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RecommendedRecipeName));
    }
}