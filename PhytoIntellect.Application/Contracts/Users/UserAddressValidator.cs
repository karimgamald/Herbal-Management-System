using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Users
{
    public class ManageUserAddressValidator : AbstractValidator<UpdateUserAddressRequest>
    {
        public ManageUserAddressValidator()
        {
            RuleFor(x => x.Governorate)
                .NotEmpty();

            RuleFor(x => x.City)
                .NotEmpty();

            RuleFor(x => x.Street)
                .NotEmpty();
        }
    }
}
