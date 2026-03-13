using FluentValidation;
using PhytoIntellect.Application.Contracts.Inventory;

namespace PhytoIntellect.Application.Validators.Inventory;

public class AddHerbToInventoryRequestValidator : AbstractValidator<AddHerbToInventoryRequest>
{
    public AddHerbToInventoryRequestValidator()
    {
        RuleFor(x => x.HerbId)
            .GreaterThan(0)
            .WithMessage("Invalid Herb Id.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.")
            .LessThan(100000)
            .WithMessage("Price is too large.");
    }
}