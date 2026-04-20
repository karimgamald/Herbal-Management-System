using FluentValidation;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Core.Enums;
using System.Linq;

namespace PhytoIntellect.Application.Contracts.Orders;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.ShippingAddress)
        .MinimumLength(10)
        .When(req => !string.IsNullOrWhiteSpace(req.ShippingAddress) && req.ShippingAddress.Trim().ToLower() != "string")
        .WithMessage("Please provide a detailed shipping address (at least 10 characters).");

        // Apply nested validators
        RuleForEach(x => x.Herbs).SetValidator(new OrderHerbRequestValidator());
        RuleForEach(x => x.Recipes).SetValidator(new OrderRecipeRequestValidator());
        RuleForEach(x => x.AiRecipes).SetValidator(new OrderAiRecipeRequestValidator());

        // Business Rule: The order must not be empty
        RuleFor(x => x)
            .Must(HaveAtLeastOneItem)
            .WithMessage("Cannot create an empty order. You must add at least one recipe, one herb, or one AI recipe.");

        RuleFor(x => x.PaymentMethod)
         .NotEmpty().WithMessage("Payment method is required.")
         .IsEnumName(typeof(PaymentMethod), caseSensitive: false)
         .WithMessage("Invalid payment method. Please choose: Cash, CreditCard, or Wallet.");
    }

    private bool HaveAtLeastOneItem(CreateOrderRequest request)
    {
        bool hasRecipes = request.Recipes != null && request.Recipes.Any();
        bool hasHerbs = request.Herbs != null && request.Herbs.Any();
        bool hasAiRecipes = request.AiRecipes != null && request.AiRecipes.Any();

        return hasRecipes || hasHerbs || hasAiRecipes;
    }
}