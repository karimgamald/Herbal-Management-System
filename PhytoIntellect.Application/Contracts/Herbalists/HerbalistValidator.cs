using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Herbalists
{
    public class HerbalistValidator : AbstractValidator<CreateOrUpdateHerbalistRequest>
    {
        public HerbalistValidator()
        {
            RuleFor(x => x.Bio)
                 .NotEmpty()
                 .MaximumLength(1000)
                 .WithMessage("Bio must not be empty and must be less than 1000 characters.");

            RuleFor(x => x.AvailableFrom)
                .NotNull()
                .WithMessage("AvailableFrom is required.");

            RuleFor(x => x.AvailableTo)
                .NotNull()
                .WithMessage("AvailableTo is required.");

            RuleFor(x => x)
                .Must(x => x.AvailableFrom != null && x.AvailableTo != null && x.AvailableTo > x.AvailableFrom)
                .WithMessage("AvailableTo must be later than AvailableFrom.");
        }
    }
}
