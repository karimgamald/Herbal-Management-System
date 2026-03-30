using PhytoIntellect.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

//public record CreateOrderRequest(
//    string ShippingAddress,
//    string PaymentMethod,
//    List<OrderRecipeRequest>? Recipes,
//    List<OrderHerbRequest>? Herbs
//);
public record CreateOrderRequest
{
    public string ShippingAddress { get; init; } = string.Empty;

    public string PaymentMethod { get; init; } = string.Empty;

    public List<OrderRecipeRequest> Recipes { get; init; } = [];
    public List<OrderHerbRequest> Herbs { get; init; } = [];
}