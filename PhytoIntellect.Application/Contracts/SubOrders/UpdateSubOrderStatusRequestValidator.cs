
using FluentValidation;
using PhytoIntellect.Application.Contracts.SubOrders;

public class UpdateSubOrderStatusRequestValidator : AbstractValidator<UpdateSubOrderStatusRequest>
{
    public UpdateSubOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(BeAValidStatus).WithMessage("Invalid status. Allowed values: Accepted, Rejected, Preparing, Shipped, Delivered");

    }

    private bool BeAValidStatus(string status)
    {
        var allowedStatuses = new[] { "Accepted", "Rejected", "Preparing", "Shipped", "Delivered" };
        return allowedStatuses.Contains(status);
    }
}