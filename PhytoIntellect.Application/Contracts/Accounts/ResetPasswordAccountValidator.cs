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

            // OldPassword اختياري، لا نتحقق منه إلا إذا كان موجود
            When(x => !string.IsNullOrWhiteSpace(x.OldPassword), () =>
            {
                RuleFor(x => x.OldPassword)
                .MinimumLength(6).WithMessage("Old password must be at least 6 characters.");
            });
        }
    }
}
