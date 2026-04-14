using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.AiRecipes;

public class CreateAiRecipeRequest
{
    public double WeightKg { get; set; }
    public double HeightCm { get; set; }
    public int SeverityScore { get; set; }
    public int SystolicBp { get; set; }
    public int DiastolicBp { get; set; }
    public double TemperatureCelsius { get; set; }
    public int HeartRateBpm { get; set; }
    public int SymptomDurationDays { get; set; }
    public List<string> SelectedSymptoms { get; set; } = [];
}
