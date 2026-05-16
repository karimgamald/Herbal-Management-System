using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.HerbalistAiChatRecipes;

public class AddAiChatRecipeToInventoryRequestValidator : AbstractValidator<AddAiChatRecipeToInventoryRequest>
{
    public AddAiChatRecipeToInventoryRequestValidator()
    {
        RuleFor(x => x.AiChatRecipeId)
            .NotEmpty().WithMessage("AI Chat Recipe ID is required.")
            .GreaterThan(0).WithMessage("Invalid AI Chat Recipe ID.");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}