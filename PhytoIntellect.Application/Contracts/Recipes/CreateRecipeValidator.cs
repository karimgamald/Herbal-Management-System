using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Recipes;

public class CreateRecipeValidator : AbstractValidator<CreateRecipeRequest>
{
    public CreateRecipeValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("Instructions are required.");

        RuleFor(x => x.Herbs)
            .NotEmpty().WithMessage("A recipe must contain at least one herb.")
            .Must(h => h != null && h.Count > 0).WithMessage("Herbs list cannot be empty.");

        // فحص كل عشبة جوه اللستة
        RuleForEach(x => x.Herbs).ChildRules(herb =>
        {
            herb.RuleFor(h => h.HerbId)
                .GreaterThan(0).WithMessage("Valid Herb ID is required.");

            herb.RuleFor(h => h.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}