using AutoMapper;
using PhytoIntellect.Application.Contracts.HerbalistAiChatRecipes;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class HerbalistAiChatRecipeProfile : Profile
{
    public HerbalistAiChatRecipeProfile()
    {
        CreateMap<HerbalistAiChatRecipe, HerbalistAiChatRecipeResponse>()
            .ForMember(dest => dest.RecommendedRecipeName, opt => opt.MapFrom(src => src.AiChatRecipe.RecommendedRecipeName))
            .ForMember(dest => dest.MainHerb, opt => opt.MapFrom(src => src.AiChatRecipe.MainHerb))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.AiChatRecipe.Category));

        CreateMap<HerbalistAiChatRecipe, HerbalistWithAiChatRecipeResponse>()
            .ForMember(dest => dest.HerbalistName, opt => opt.MapFrom(src => src.Herbalist.User!.FullName))
            .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.Herbalist.LicenseNumber))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Herbalist.AverageRating));
    }
}