using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public class OrderAiChatRecipeRequestValidator : AbstractValidator<OrderAiChatRecipeRequest>
{
    public OrderAiChatRecipeRequestValidator()
    {
        RuleFor(x => x.AiChatRecipeId)
            .GreaterThan(0).WithMessage("Invalid Ai Recipe ID. It must be greater than zero.");

        RuleFor(x => x.HerbalistId)
            .GreaterThan(0).WithMessage("Herbalist ID is required to purchase this herb.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}
