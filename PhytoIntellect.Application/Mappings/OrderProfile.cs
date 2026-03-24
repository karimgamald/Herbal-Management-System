using AutoMapper;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Core.Entities;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderSummaryResponse>();
        CreateMap<Order, OrderDetailsResponse>();
    }
}