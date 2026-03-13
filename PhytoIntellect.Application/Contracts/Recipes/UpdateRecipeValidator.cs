using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Recipes;

public class UpdateRecipeValidator : AbstractValidator<UpdateRecipeRequest>
{
    public UpdateRecipeValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Instructions).NotEmpty();
        RuleFor(x => x.Herbs).NotEmpty().Must(h => h != null && h.Count > 0);

        RuleForEach(x => x.Herbs).ChildRules(herb =>
        {
            herb.RuleFor(h => h.HerbId).GreaterThan(0);
            herb.RuleFor(h => h.Quantity).GreaterThan(0);
        });
    }
}