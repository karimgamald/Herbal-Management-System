using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class AiRecipe
{
    [Key]
    public int Id { get; set; }

    public int PatientId { get; set; }
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; }

    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public double WeightKg { get; set; }
    public double HeightCm { get; set; }
    public double Bmi { get; set; }
    public int SeverityScore { get; set; }
    public int SystolicBp { get; set; }
    public int DiastolicBp { get; set; }
    public double TemperatureCelsius { get; set; }
    public int HeartRateBpm { get; set; }
    public int SymptomDurationDays { get; set; }

    public bool HasDiabetes { get; set; }
    public bool HasHypertension { get; set; }
    public bool HasAllergies { get; set; }
    public bool IsPregnant { get; set; }
    public bool IsSmoker { get; set; }

    public List<string> Symptoms { get; set; } = [];


    public string RecommendedRecipeName { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }

    public List<string> PreparationInstructions { get; set; } = [];
    public string CautionWarning { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public float? Rating { get; set; }

    public float HerbalistAverageRating { get; set; } = 0;
    public int HerbalistTotalRatings { get; set; } = 0;

    // Navigation Properties 
    public ICollection<Feedback> Feedbacks { get; set; } = [];
    public ICollection<ReviewRecipe> HerbalistReviews { get; set; } = [];
    public ICollection<HerbalistAiRecipe> HerbalistInventories { get; set; } = [];
}
