using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.AiRecipes;

public class CreateAiRecipeRequest
{
    public int PatientId { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public double WeightKg { get; set; }
    public double HeightCm { get; set; }
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

    // الفلاتر هيبعتلك الأعراض كلستة جاهزة لتسهيل الدنيا عليه وعليك
    public List<string> SelectedSymptoms { get; set; } = new List<string>();
}