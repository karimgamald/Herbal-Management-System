using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public class OrderRecipeRequestValidator : AbstractValidator<OrderRecipeRequest>
{
    public OrderRecipeRequestValidator()
    {
        // RecipeId
        RuleFor(x => x.RecipeId)
            .NotNull().WithMessage("Recipe ID is required.")
            .GreaterThan(0).WithMessage("Invalid recipe ID. It must be greater than zero.")
            .When(x => x.RecipeId.HasValue);

        // Quantity
        RuleFor(x => x.Quantity)
            .NotNull().WithMessage("Quantity is required.")
            .GreaterThan(0).WithMessage("Quantity must be at least 1.")
            .When(x => x.Quantity.HasValue);
    }
}