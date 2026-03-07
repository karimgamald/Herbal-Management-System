using FluentValidation;


namespace PhytoIntellect.Application.Contracts.Patients;

public class PatientValidator: AbstractValidator<PatientRequest>
{
    public PatientValidator()
    {
        //RuleFor(x => x.)
        //    .NotEmpty()
        //    .Length(3, 100);
    }
}
