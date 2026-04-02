using AutoMapper;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class OrderRecipeProfile: Profile
{
    public OrderRecipeProfile()
    {
        CreateMap<OrderRecipe, OrderRecipeResponse>()
            .ForMember(dest => dest.RecipeName, opt => opt.MapFrom(src => src.Recipe!.Description))
            .ForMember(dest => dest.QuantityPerOne, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPricePerOne, opt => opt.MapFrom(src => src.UnitPrice));
    }
}
