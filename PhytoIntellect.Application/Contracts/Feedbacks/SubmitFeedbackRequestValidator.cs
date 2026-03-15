using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Feedbacks;

public class SubmitFeedbackRequestValidator : AbstractValidator<SubmitFeedbackRequest>
{
    public SubmitFeedbackRequestValidator()
    {
        RuleFor(x => x.RatingValue)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
    }
}