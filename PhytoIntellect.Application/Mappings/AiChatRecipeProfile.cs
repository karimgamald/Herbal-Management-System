using AutoMapper;
using PhytoIntellect.Application.Contracts.ChatAiRecipes;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings;

public class AiChatRecipeProfile : Profile
{
    public AiChatRecipeProfile()
    {
        CreateMap<AiChatRecipe, AiChatPredictionResult>();
    }
}