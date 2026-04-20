using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public record OrderAiRecipeRequest
{
    public int HerbalistId { get; set; }
    public int AiRecipeId { get; set; }
    public int Quantity { get; set; }
}