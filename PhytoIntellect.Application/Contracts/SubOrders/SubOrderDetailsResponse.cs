using PhytoIntellect.Application.Contracts.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.SubOrders;

public record SubOrderDetailsResponse
{
    public int SubOrderId { get; init; }
    public string HerbalistName { get; init; } = string.Empty;
    public decimal SubTotal { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? TrackingNumber { get; init; }

    public List<OrderHerbResponse> Herbs { get; init; } = new();
    public List<OrderRecipeResponse> Recipes { get; init; } = new();
    public List<OrderAiRecipeResponse> AiRecipes { get; init; } = new();
}