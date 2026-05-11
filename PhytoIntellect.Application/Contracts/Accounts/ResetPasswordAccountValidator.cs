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
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6);

            // ✅ if using old password flow
            When(x => string.IsNullOrWhiteSpace(x.Token), () =>
            {
                RuleFor(x => x.OldPassword)
                    .NotEmpty()
                    .WithMessage("Old password is required when token is not provided.");
            });

            // ✅ if using token flow
            When(x => string.IsNullOrWhiteSpace(x.OldPassword), () =>
            {
                RuleFor(x => x.Token)
                    .NotEmpty()
                    .WithMessage("Reset token is required when old password is not provided.");
            });
        }
    }
}
