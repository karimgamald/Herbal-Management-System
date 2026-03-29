using FluentValidation;
using PhytoIntellect.Application.Contracts.Orders;

public class OrderHerbRequestValidator : AbstractValidator<OrderHerbRequest>
{
    public OrderHerbRequestValidator()
    {
        // HerbId
        RuleFor(x => x.HerbId)
            .NotNull().WithMessage("Herb ID is required.")
            .GreaterThan(0).WithMessage("Invalid herb ID. It must be greater than zero.")
            .When(x => x.HerbId.HasValue);

        // HerbalistId
        RuleFor(x => x.HerbalistId)
            .NotNull().WithMessage("Herbalist ID is required.")
            .GreaterThan(0).WithMessage("Herbalist ID must be greater than zero.")
            .When(x => x.HerbalistId.HasValue);

        // Quantity
        RuleFor(x => x.Quantity)
            .NotNull().WithMessage("Quantity is required.")
            .GreaterThan(0).WithMessage("Quantity must be at least 1.")
            .When(x => x.Quantity.HasValue);
    }
}