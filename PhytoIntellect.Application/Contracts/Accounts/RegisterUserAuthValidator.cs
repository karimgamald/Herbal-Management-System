using FluentValidation;

namespace PhytoIntellect.Application.Contracts.Accounts
{
    public class RegisterUserAuthValidator : AbstractValidator<RegisterUserAuthRequest>
    {
        public RegisterUserAuthValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d).*$")
                .WithMessage("Password must contain at least one letter and one number.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.Password)
                .WithMessage("Password and Confirm Password must match.");

            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(role => role == "Patient" || role == "Herbalist" || role == "Admin")
                .WithMessage("Role must be Patient or Herbalist.");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^01[0-2,5]{1}[0-9]{8}$")
                .WithMessage("Phone number is not valid.");
        }
    }
}
