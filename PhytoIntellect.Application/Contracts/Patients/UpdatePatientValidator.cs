using FluentValidation;


namespace PhytoIntellect.Application.Contracts.Patients;

public class UpdatePatientValidator: AbstractValidator<UpdatePatientRequest>
{
    public UpdatePatientValidator()
    {
        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Birth date is required.")
            .Must(BeAValidDate).WithMessage("Invalid date format. Please use YYYY-MM-DD.")
            .Must(BeInThePast).WithMessage("Birth date cannot be in the future.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(BeAValidGender).WithMessage("Gender must be 'Male' or 'Female'.");
    }

    private bool BeAValidDate(string dateString)
    {
        return DateOnly.TryParse(dateString, out _);
    }

    private bool BeInThePast(string dateString)
    {
        if (DateOnly.TryParse(dateString, out var parsedDate))
        {
            return parsedDate <= DateOnly.FromDateTime(DateTime.Now);
        }
        return false;
    }

    private bool BeAValidGender(string gender)
    {
        if (string.IsNullOrWhiteSpace(gender)) return false;
        var g = gender.ToLower();
        return g == "male" || g == "female";
    }
}
