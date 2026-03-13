using FluentValidation;
using PhytoIntellect.Application.Contracts.Herbs;

namespace PhytoIntellect.Application.Validators.Herbs;

public class HerbRequestValidator : AbstractValidator<HerbRequest>
{
    public HerbRequestValidator()
    {
        RuleFor(x => x.HerbName)
            .NotEmpty()
            .WithMessage("Herb name is required.")
            .MaximumLength(100)
            .WithMessage("Herb name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Benefits)
            .NotEmpty()
            .WithMessage("Benefits are required.")
            .MaximumLength(1000);

        RuleFor(x => x.Warnings)
            .MaximumLength(1000);
    }
}