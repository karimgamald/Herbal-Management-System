using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class Recipe
{
    public int RecipeId { get; set; }

    public int? HerbalistId { get; set; }

    public bool CreatedByAI { get; set; } = true;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public decimal Price { get; set; }
    public float AverageRating { get; set; }
    public int TotalRatings { get; set; } = 0;
    
    
    // تقييمات العطارين (مفصولين عن المرضى)
    public float HerbalistAverageRating { get; set; } = 0;
    public int HerbalistTotalRatings { get; set; } = 0;

    // العلاقة
    public ICollection<ReviewRecipe> Reviews { get; set; } = [];

    // Navigation Properties
    public Herbalist? Herbalist { get; set; }
    public ICollection<RecipeHerb> RecipeHerbs { get; set; } = [];
    public ICollection<RecipeDisease> RecipeDiseases { get; set; } = [];
    public ICollection<Feedback> Feedbacks { get; set; } = [];
}