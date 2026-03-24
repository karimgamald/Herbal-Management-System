using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public record CreateOrderRequest(
    string ShippingAddress,
    string PaymentMethod,
    List<OrderRecipeRequest>? Recipes,
    List<OrderHerbRequest>? Herbs
);