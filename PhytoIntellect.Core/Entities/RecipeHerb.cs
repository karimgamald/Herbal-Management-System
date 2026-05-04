using System;
using System.Collections.Generic;
using System.Text;
namespace PhytoIntellect.Core.Entities;

public class RecipeHerb
{
    public int RecipeHerbId { get; set; }
    public int RecipeId { get; set; }
    public int HerbId { get; set; }

    public float Quantity { get; set; }

    public Recipe Recipe { get; set; } = null!;
    public Herb Herb { get; set; } = null!;
}
