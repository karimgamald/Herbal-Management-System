using PhytoIntellect.Infrastructure.ExternalApi.AiContracts;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.ExternalApi;

public interface IAiFlaskClient
{
    [Post("/api/predict")]
    Task<FlaskAiResponse> GetPredictionAsync([Body] FlaskAiRequest request);
}