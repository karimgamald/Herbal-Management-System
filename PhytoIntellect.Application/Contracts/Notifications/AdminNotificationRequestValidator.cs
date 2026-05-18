using FluentValidation;
using PhytoIntellect.Application.Contracts.Notifications;
using PhytoIntellect.Core.Constants;
using System;

namespace PhytoIntellect.Application.Validators.Notifications;

public class AdminNotificationRequestValidator : AbstractValidator<AdminNotificationRequest>
{
    public AdminNotificationRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Notification title is required.")
            .MaximumLength(150).WithMessage("Title is too long. Maximum length is 150 characters.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Notification message body is required.")
            .MaximumLength(1000).WithMessage("Message body is too long. Maximum length is 1000 characters.");

        RuleFor(x => x.TargetRole)
            .NotEmpty().WithMessage("Target role must be specified.")
            .Must(role => role == AppRoles.Herbalist ||
                          role == AppRoles.Patient ||
                          role.Equals("All", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Invalid target role. Allowed values are 'Herbalist', 'Patient', or 'All'.");
    }
}