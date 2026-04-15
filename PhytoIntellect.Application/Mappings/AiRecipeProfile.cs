using AutoMapper;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Application.Contracts.AiRecipes;

public class AiRecipeProfile : Profile
{
    public AiRecipeProfile()
    {
        CreateMap<AiRecipe, AiRecipeResponse>()
            .ForMember(dest => dest.RecipeId,
                       opt => opt.MapFrom(src => src.Id));
    }
}