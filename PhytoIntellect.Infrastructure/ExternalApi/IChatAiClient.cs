using PhytoIntellect.Infrastructure.ExternalApi.ChatContracts;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.ExternalApi;

public interface IChatAiClient
{
    [Post("/predict")]
    Task<FlaskChatAiResponse> GetChatPredictionAsync([Body] FlaskChatAiRequest request, CancellationToken cancellationToken = default);
}