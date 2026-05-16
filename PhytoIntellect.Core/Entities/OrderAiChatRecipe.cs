using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class OrderAiChatRecipe
{
    public int OrderAiChatRecipeId { get; set; }
    public int SubOrderId { get; set; }
    public int AiChatRecipeId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }

    // Navigation Properties
    public SubOrder? SubOrder { get; set; }
    public AiChatRecipe? AiChatRecipe { get; set; }
} 