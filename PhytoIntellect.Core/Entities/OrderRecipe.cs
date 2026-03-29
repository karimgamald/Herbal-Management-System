using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class OrderRecipe
{
    public int OrderRecipeId { get; set; }
    public int SubOrderId { get; set; }
    public int? RecipeId { get; set; }

    public int? Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? SubTotal { get; set; }

    // Navigation Properties
    public SubOrder? SubOrder { get; set; }
    public Recipe? Recipe { get; set; }
}