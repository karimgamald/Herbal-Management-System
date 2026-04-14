using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.AiRecipes;

public class CreateAiRecipeValidator : AbstractValidator<CreateAiRecipeRequest>
{
    public CreateAiRecipeValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0).WithMessage("Patient ID is required.");

        RuleFor(x => x.Age).InclusiveBetween(1, 120).WithMessage("Age must be between 1 and 120.");

        RuleFor(x => x.Gender).NotEmpty().Must(g => g.ToLower() == "male" || g.ToLower() == "female")
            .WithMessage("Gender must be Male or Female.");

        RuleFor(x => x.WeightKg).GreaterThan(20).WithMessage("Weight must be valid.");

        RuleFor(x => x.SelectedSymptoms)
            .NotEmpty().WithMessage("At least one symptom must be selected.")
            .Must(list => list.Count <= 15).WithMessage("Cannot select more than 15 symptoms at once.");
    }
}