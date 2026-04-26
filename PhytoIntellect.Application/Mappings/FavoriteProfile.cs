using AutoMapper;
using PhytoIntellect.Application.Contracts.UserFavorites;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class FavoriteProfile : Profile
{
    public FavoriteProfile()
    {
        CreateMap<Herb, FavoriteResponse>()
            .ForMember(dest => dest.TargetId, opt => opt.MapFrom(src => src.HerbId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.HerbName));

        CreateMap<Recipe, FavoriteResponse>()
            .ForMember(dest => dest.TargetId, opt => opt.MapFrom(src => src.RecipeId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.Description) ? "Recipe without Name" :
                src.Description.Length > 50 ? src.Description.Substring(0, 50) + "..." : src.Description));

        CreateMap<AiRecipe, FavoriteResponse>()
            .ForMember(dest => dest.TargetId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.Condition)
                    ? "AI Generated Recipe"
                    : $"AI Recipe for {src.Condition}"));

        CreateMap<Herbalist, FavoriteResponse>()
            .ForMember(dest => dest.TargetId, opt => opt.MapFrom(src => src.HerbalistId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User!.FullName));
    }
}