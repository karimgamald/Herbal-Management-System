using AutoMapper;
using PhytoIntellect.Application.Contracts.HerbalistAiRecipes;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class HerbalistAiRecipeProfile : Profile
{
    public HerbalistAiRecipeProfile()
    {
        CreateMap<AddAiRecipeToInventoryRequest, HerbalistAiRecipe>();

        CreateMap<HerbalistAiRecipe, HerbalistAiRecipeResponse>()
            .ForMember(dest => dest.RecipeName,
                       opt => opt.MapFrom(src => src.AiRecipe.RecommendedRecipeName));

        CreateMap<HerbalistAiRecipe, HerbalistWithAiRecipeResponse>()
            .ForMember(dest => dest.HerbalistName, opt => opt.MapFrom(src => src.Herbalist.User!.FullName))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                $"{src.Herbalist.User!.Governorate} - {src.Herbalist.User.City} - {src.Herbalist.User.Street}"));
    }
}