using FluentValidation;


namespace PhytoIntellect.Application.Contracts.Patients;

public class ManagePatientValidator: AbstractValidator<ManagePatientRequest>
{
    public ManagePatientValidator()
    {
        //RuleFor(x => x.)
        //    .NotEmpty()
        //    .Length(3, 100);
    }
}
