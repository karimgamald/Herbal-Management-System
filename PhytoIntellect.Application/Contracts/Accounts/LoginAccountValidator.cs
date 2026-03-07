using FluentValidation;
using PhytoIntellect.Application.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Accounts
{
    public class LoginAccountValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginAccountValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
