using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class HerbalistAiRecipe
{
    // Composite Key
    public int HerbalistId { get; set; }
    public int AiRecipeId { get; set; }

    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Herbalist Herbalist { get; set; } = null!;
    public AiRecipe AiRecipe { get; set; } = null!;
}