using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Reviews;

public record SubmitReviewRequest
{
    public float RatingValue { get; init; }
    public string? Comment { get; init; }
}
