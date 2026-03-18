using PhytoIntellect.Application.Contracts.Diseases;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IDiseaseService
{
    Task<IEnumerable<DiseaseResponse>> GetAllDiseasesAsync(CancellationToken cancellationToken = default);
}