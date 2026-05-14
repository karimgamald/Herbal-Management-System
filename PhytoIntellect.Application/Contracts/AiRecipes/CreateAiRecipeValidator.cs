using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.AiRecipes;

public class CreateAiRecipeValidator : AbstractValidator<CreateAiRecipeRequest>
{
    public static readonly HashSet<string> AllowedSymptoms = new(StringComparer.OrdinalIgnoreCase)
    {
        "Itchy Eyes", "Visual Aura", "Runny Nose", "Headache", "Watery Eyes", "Sneezing",
        "Chest Pain", "Painful Urination", "Pelvic Pain", "Severe Headache", "Light Sensitivity",
        "Cloudy Urine", "Abdominal Pain", "Vomiting", "Fever", "Dizziness", "Cold Hands",
        "Cough", "Slow Healing", "Cold Intolerance", "Blurred Vision", "Diarrhea", "Weight Gain",
        "Nausea", "Excessive Thirst", "Nasal Congestion", "Frequent Urination", "Dry Skin",
        "Pale Skin", "Fatigue", "Constipation", "Sore Throat", "Shortness Of Breath"
    };

    public CreateAiRecipeValidator()
    {
        // 1. Weight Validation
        RuleFor(x => x.WeightKg)
            .InclusiveBetween(20, 300)
            .WithMessage("Weight must be between 20 and 300 Kg.");

        // 2. Height Validation
        RuleFor(x => x.HeightCm)
            .InclusiveBetween(100, 250)
            .WithMessage("Height must be between 100 and 250 cm.");

        // 3. Severity Score Validation
        RuleFor(x => x.SeverityScore)
            .InclusiveBetween(1, 10)
            .WithMessage("Severity score must be a number between 1 and 10.");

        // 4. Systolic Blood Pressure Validation
        RuleFor(x => x.SystolicBp)
            .InclusiveBetween(60, 250)
            .WithMessage("Systolic blood pressure must be between 60 and 250 mmHg.");

        // 5. Diastolic Blood Pressure Validation
        RuleFor(x => x.DiastolicBp)
            .InclusiveBetween(40, 150)
            .WithMessage("Diastolic blood pressure must be between 40 and 150 mmHg.");

        // 6. Temperature Validation
        RuleFor(x => x.TemperatureCelsius)
            .InclusiveBetween(35, 42)
            .WithMessage("Body temperature must be between 35 and 42 °C.");

        // 7. Heart Rate Validation
        RuleFor(x => x.HeartRateBpm)
            .InclusiveBetween(30, 220)
            .WithMessage("Heart rate must be between 30 and 220 bpm.");

        // 8. Symptom Duration Validation
        RuleFor(x => x.SymptomDurationDays)
            .InclusiveBetween(0, 365)
            .WithMessage("Symptom duration must be between 0 and 365 days.");

        // 9. Symptoms Validation
        RuleFor(x => x.SelectedSymptoms)
            .NotEmpty().WithMessage("At least one symptom must be selected.")
            .Must(list => list != null && list.Count <= 15)
            .WithMessage("Cannot select more than 15 symptoms at once.")
            .Must(HaveValidSymptoms)
            .WithMessage("One or more selected symptoms are invalid. Please choose from the provided list.");
    }

    private bool HaveValidSymptoms(List<string> symptoms)
    {
        if (symptoms == null || !symptoms.Any()) return false;

        foreach (var symptom in symptoms)
        {
            if (!AllowedSymptoms.Contains(symptom.Trim()))
            {
                return false;
            }
        }
        return true;
    }
}