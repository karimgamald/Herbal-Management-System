using AutoMapper;
using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class RecipeProfile : Profile
{
    public RecipeProfile()
    {
        // Request -> Entity (عشان نسيف في الداتابيز)
        CreateMap<CreateRecipeRequest, Recipe>()
            .ForMember(dest => dest.CreatedByAI, opt => opt.MapFrom(src => false)) // بما إن العطار اللي بيكريت
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.RecipeHerbs, opt => opt.MapFrom(src => src.Herbs));

        CreateMap<RecipeHerbRequest, RecipeHerb>();

        // Entity -> Response (عشان نرجعها للـ User)
        CreateMap<Recipe, RecipeResponse>()
            .ForMember(dest => dest.Herbs, opt => opt.MapFrom(src => src.RecipeHerbs));

        // دي التريكة اللي بتجيب اسم العشبة من جدول الـ Herb
        CreateMap<RecipeHerb, RecipeHerbResponse>()
            .ForMember(dest => dest.HerbName, opt => opt.MapFrom(src => src.Herb.HerbName));
    }
}