using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.ShippingAddress)
        .MinimumLength(10)
        .When(req => !string.IsNullOrWhiteSpace(req.ShippingAddress) && req.ShippingAddress.Trim().ToLower() != "string")
        .WithMessage("Please provide a detailed shipping address (at least 10 characters).");

        // Payment Method Validation
        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required.");

        // Apply nested validators
        RuleForEach(x => x.Recipes).SetValidator(new OrderRecipeRequestValidator());
        RuleForEach(x => x.Herbs).SetValidator(new OrderHerbRequestValidator());

        // Business Rule: The order must not be empty
        RuleFor(x => x)
            .Must(HaveAtLeastOneItem)
            .WithMessage("Cannot create an empty order. You must add at least one recipe or one herb.");
    }

    private bool HaveAtLeastOneItem(CreateOrderRequest request)
    {
        bool hasRecipes = request.Recipes != null && request.Recipes.Any();
        bool hasHerbs = request.Herbs != null && request.Herbs.Any();

        return hasRecipes || hasHerbs;
    }
}