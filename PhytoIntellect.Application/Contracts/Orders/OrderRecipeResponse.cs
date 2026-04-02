using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public record OrderRecipeResponse
{
    public int RecipeId { get; init; }
    public string RecipeName { get; init; } = string.Empty;
    public int QuantityPerOne { get; init; }
    public decimal UnitPricePerOne { get; init; }
    public decimal SubTotal { get; init; }
}