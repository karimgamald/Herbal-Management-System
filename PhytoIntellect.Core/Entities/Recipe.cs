using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class Recipe
{
    public int RecipeId { get; set; }

    // Nullable عشان لو الوصفة معمولة بالذكاء الاصطناعي مش هيكون ليها عطار محدد
    public int? HerbalistId { get; set; }

    public bool CreatedByAI { get; set; } = true;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public float AverageRating { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    [ForeignKey(nameof(HerbalistId))]
    public Herbalist? Herbalist { get; set; }
    public ICollection<RecipeHerb> RecipeHerbs { get; set; } = [];
}