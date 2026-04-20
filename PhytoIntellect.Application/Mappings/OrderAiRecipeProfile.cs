using AutoMapper;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class OrderAiRecipeProfile : Profile
{
    public OrderAiRecipeProfile()
    {
        CreateMap<OrderAiRecipe, OrderAiRecipeResponse>()
            .ForMember(dest => dest.RecipeName, opt => opt.MapFrom(src => src.AiRecipe!.RecommendedRecipeName));
        
    }
}