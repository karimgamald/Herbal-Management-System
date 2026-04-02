using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public record OrderHerbResponse
{
    public int HerbId { get; init; }
    public string HerbName { get; init; } = string.Empty;
    public int QuantityPerGram { get; init; }
    public decimal UnitPricePerKilo { get; init; }
    public decimal SubTotal { get; init; }
}