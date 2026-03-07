using FluentValidation;
using PhytoIntellect.Application.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Accounts
{
    public class ForgetPasswordValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        }
    }
}
