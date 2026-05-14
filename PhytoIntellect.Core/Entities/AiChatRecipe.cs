using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class AiChatRecipe : LocalizedEntity
{
    [Key]
    public int Id { get; set; }
    public int PatientId { get; set; }
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; }
    public string UserPrompt { get; set; } = string.Empty;
    public string RecommendedRecipeName { get; set; } = string.Empty;
    public string MainHerb { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Preparation { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Contraindications { get; set; } = string.Empty; 
    public double MatchPercentage { get; set; } 
    public List<string> OtherPossibilities { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true; 
    public bool IsAvailable { get; set; } = false;

    // 4. Rating System
    //public float? Rating { get; set; }
    //public float HerbalistAverageRating { get; set; } = 0;
    //public int HerbalistTotalRatings { get; set; } = 0;

    // 5. Navigation Properties
    //public ICollection<Feedback> Feedbacks { get; set; } = [];
    //public ICollection<ReviewRecipe> HerbalistReviews { get; set; } = [];
    //public ICollection<HerbalistAiRecipe> HerbalistInventories { get; set; } = [];
}