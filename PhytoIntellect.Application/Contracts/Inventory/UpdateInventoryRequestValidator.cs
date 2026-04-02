using FluentValidation;
using PhytoIntellect.Application.Contracts.Inventory;

namespace PhytoIntellect.Application.Validators.Inventory;

public class UpdateInventoryRequestValidator : AbstractValidator<UpdateInventoryRequest>
{
    public UpdateInventoryRequestValidator()
    {
        RuleFor(x => x.PricePerKilo)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.")
            .LessThan(1000)
            .WithMessage("Price is too large.");
    }
}