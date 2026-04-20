using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class OrderAiRecipe
{
    public int OrderAiRecipeId { get; set; }

    public int SubOrderId { get; set; }
    public int AiRecipeId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }

    // Navigation Properties
    public SubOrder? SubOrder { get; set; }
    public AiRecipe? AiRecipe { get; set; }
} 