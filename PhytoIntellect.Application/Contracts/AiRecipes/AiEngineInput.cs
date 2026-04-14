using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.AiRecipes;

public class AiEngineInput
{
    public int Age { get; set; }
    public string Gender { get; set; }
    public bool HasDiabetes { get; set; }
    public bool HasHypertension { get; set; }
    public bool HasAllergies { get; set; }
    public bool IsPregnant { get; set; }
    public bool IsSmoker { get; set; }

    public CreateAiRecipeRequest CurrentVitals { get; set; }
}