using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Feedbacks;

public record SubmitFeedbackRequest
{
    public float RatingValue { get; init; }
    public string? Comment { get; init; }
}
