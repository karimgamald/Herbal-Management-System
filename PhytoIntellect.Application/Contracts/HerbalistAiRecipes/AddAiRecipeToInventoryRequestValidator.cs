using FluentValidation;
using PhytoIntellect.Application.Contracts.Inventory;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.HerbalistAiRecipes;

public class AddAiRecipeToInventoryRequestValidator : AbstractValidator<AddAiRecipeToInventoryRequest>
{
    public AddAiRecipeToInventoryRequestValidator()
    {
        RuleFor(x => x.AiRecipeId)
            .GreaterThan(0)
            .WithMessage("Invalid Herb Id.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.")
            .LessThan(100000)
            .WithMessage("Price is too large.");
    }
}