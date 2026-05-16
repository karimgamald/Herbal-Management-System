using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public class OrderAiChatRecipeResponse
{
    public int AiChatRecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}