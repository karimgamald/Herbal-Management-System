using AutoMapper;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class OrderHerbProfile : Profile
{
    public OrderHerbProfile()
    {
        CreateMap<OrderHerb, OrderHerbResponse>()
            .ForMember(dest => dest.HerbName, opt => opt.MapFrom(src => src.Herb!.HerbName))
            .ForMember(dest => dest.QuantityPerGram, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPricePerKilo, opt => opt.MapFrom(src => src.UnitPrice));
    }
}
