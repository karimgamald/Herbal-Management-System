using FluentValidation;
using PhytoIntellect.Application.Contracts.MedicalHistories;
using System.Text.RegularExpressions;

namespace PhytoIntellect.Application.Contracts.MedicalHistory;

public class ManageMedicalHistoryValidator : AbstractValidator<MedicalHistoryRequest>
{
    public ManageMedicalHistoryValidator()
    {
        RuleFor(x => x.OtherNotes)
            .Must(ContainValidText!)
            .WithMessage("Other notes must contain actual text (letters or numbers), not just symbols.")
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.") 
            .When(x => !string.IsNullOrWhiteSpace(x.OtherNotes));
    }

    private bool ContainValidText(string notes)
    {
        var regex = new Regex(@"[a-zA-Z\u0600-\u06FF]");

        return regex.IsMatch(notes);
    }
}