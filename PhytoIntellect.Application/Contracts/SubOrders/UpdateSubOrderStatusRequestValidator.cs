
using FluentValidation;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Core.Enums;

public class UpdateSubOrderStatusRequestValidator : AbstractValidator<UpdateSubOrderStatusRequest>
{
    public UpdateSubOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(BeAValidStatus).WithMessage("Invalid status. Allowed values: Preparing, Shipped, Delivered, Cancelled");
    }

    private bool BeAValidStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status)) return false;

        var allowedStatuses = new[]
        {
            SubOrderStatus.Preparing.ToString(),
            SubOrderStatus.Shipped.ToString(),
            SubOrderStatus.Delivered.ToString(),
            SubOrderStatus.Cancelled.ToString()
        };

        return allowedStatuses.Contains(status.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}