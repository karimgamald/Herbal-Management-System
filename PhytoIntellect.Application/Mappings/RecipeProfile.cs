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
        CreateMap<CreateRecipeRequest, Recipe>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.RecipeHerbs, opt => opt.MapFrom(src => src.Herbs)) 
            .ForMember(dest => dest.RecipeDiseases, opt => opt.MapFrom(src =>       
                src.DiseaseIds != null ? src.DiseaseIds.Select(id => new RecipeDisease { DiseaseId = id }) : null));

        CreateMap<RecipeHerbRequest, RecipeHerb>();

        CreateMap<Recipe, RecipeResponse>()
            .ForMember(dest => dest.Herbs, opt => opt.MapFrom(src => src.RecipeHerbs))
            .ForMember(dest => dest.TargetedDiseases, opt => opt.MapFrom(src => src.RecipeDiseases));

        CreateMap<RecipeHerb, RecipeHerbResponse>()
            .ForMember(dest => dest.HerbName, opt => opt.MapFrom(src => src.Herb.HerbName));

        CreateMap<RecipeDisease, RecipeDiseaseResponse>()
            .ForMember(dest => dest.DiseaseName, opt => opt.MapFrom(src => src.Disease.DiseaseName));
    }
}