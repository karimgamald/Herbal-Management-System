using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public class OrderAiChatRecipeRequest
{
    public int AiChatRecipeId { get; set; }
    public int HerbalistId { get; set; }
    public int Quantity { get; set; }
}