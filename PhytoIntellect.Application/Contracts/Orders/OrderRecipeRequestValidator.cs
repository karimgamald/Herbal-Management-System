using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public class OrderRecipeRequestValidator : AbstractValidator<OrderRecipeRequest>
{
    public OrderRecipeRequestValidator()
    {
        RuleFor(x => x.RecipeId)
            .GreaterThan(0).WithMessage("Invalid recipe ID. It must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}