using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Accounts
{
    public class ResetPasswordAccountValidator : AbstractValidator<ResetPasswordAccountRequest>
    {
        public ResetPasswordAccountValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            // 🔐 Optional Old Password (for logged-in change password)
            When(x => !string.IsNullOrWhiteSpace(x.OldPassword), () =>
            {
                RuleFor(x => x.OldPassword)
                    .MinimumLength(6)
                    .WithMessage("Old password must be at least 6 characters.");
            });

            // 🔐 Optional Token (for forgot password flow)
            When(x => !string.IsNullOrWhiteSpace(x.Token), () =>
            {
                RuleFor(x => x.Token)
                    .NotEmpty()
                    .WithMessage("Reset token is required.");
            });

            // 🚨 MUST HAVE ONE OF THEM (important rule)
            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrWhiteSpace(x.OldPassword) ||
                    !string.IsNullOrWhiteSpace(x.Token))
                .WithMessage("Either OldPassword or Reset Token is required.");
        }
    }
}
