
﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public class OrderHerbRequestValidator : AbstractValidator<OrderHerbRequest>
{
    public OrderHerbRequestValidator()
    {
        RuleFor(x => x.HerbId)
            .GreaterThan(0).WithMessage("Invalid herb ID. It must be greater than zero.");

        RuleFor(x => x.HerbalistId)
            .GreaterThan(0).WithMessage("Herbalist ID is required to purchase this herb.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}